using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SampSharp.GameMode.Pools;
using MySql.Data.MySqlClient;
using Game.Core;

namespace Game.World.Property
{
    public abstract partial class Property : IdentifiedPool<Property>
    {
        private PropertyType __type;
        private DynamicArea __area = null;
        private DynamicPickup __pickup = null;
        private DynamicMapIcon __icon = null;
        private DynamicTextLabel __label = null;
        private Interior __interior = null;
        private Vector3 __pos;
        private float __angle;
        private bool __locked;
        private List<Player> __PlayersIn = new List<Player>();
        private int __deposit;
        private int __price;
        private int __owner;

        public Property(int id, PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            __Spawn(type, interior, pos, angle);
            Id = id;
        }

        public Property(PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            __Spawn(type, interior, pos, angle);

            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO properties (interior, x, y, z, a, locked) VALUES (@interior, @x, @y, @z, @a, @locked)", conn);

                int inter = Interior.GetAll<Interior>().IndexOf(interior);
                if (inter == -1)
                    cmd.Parameters.AddWithValue("@interior", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@interior", inter);

                cmd.Parameters.AddWithValue("@x", pos.X);
                cmd.Parameters.AddWithValue("@y", pos.Y);
                cmd.Parameters.AddWithValue("@z", pos.Z);
                cmd.Parameters.AddWithValue("@a", angle);
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.ExecuteNonQuery();

                Id = (int)cmd.LastInsertedId;
            }
        }

        //
        // Summary:
        //     Dispose a property.
        public virtual void Remove()
        {
            // Curatam memoria
            __pickup.Dispose();
            __label.Dispose();
            //__area.Dispose();

            if (__icon != null)
                __icon.Dispose();

            foreach (Player player in __PlayersIn)
                player.RemoveFromProperty();

            foreach (Player player in Player.GetAll<Player>().Where(p => p.PropertyInteracting == this))
               player.PropertyInteracting = null;

            foreach (Player player in Player.GetAll<Player>().Where(p => p.RentedRoom == this))
                player.RentedRoom = null;

            using (var conn = Database.Connect())
            {
                new MySqlCommand("DELETE FROM properties WHERE id="+Id, conn)
                    .ExecuteNonQuery();
            }

            base.Dispose(true);
        }

        //
        // Summary:
        //     Get the nametype of property (Ex: Home, Business etc)
        public override string ToString()
        {
            return __type.ToString().Remove(0, 4);
        }

        //
        // Summary:
        //     Gets the position of exterior of property.
        public virtual Vector3 Position
        {
            get { return __pos; }
            set
            {
                // Actualizam pozitile
                __pos = value;
                __pickup.Position = __pos;
                __label.Position = __pos;

                // Doar business-urile au iconite
                if (__type == PropertyType.TypeBusiness)
                    __icon.Position = __pos;
            }
        }

        //
        // Summary:
        //     Gets the property type.
        public virtual PropertyType Type => __type;

        //
        // Summary:
        //     Gets the pickup.
        public virtual DynamicPickup Pickup => __pickup;

        //
        // Summary:
        //     Gets the icon of property.
        public virtual DynamicMapIcon Icon => __icon;

        //
        // Summary:
        //     Gets the label of property.
        public virtual DynamicTextLabel Label => __label;

        public virtual bool ShowIcon(int id)
        {
            if(id < 0 || id > 63)
            {
                return false;
            }

            if (__icon != null)
                __icon.Type = id;
            else
                __icon = new DynamicMapIcon(__pos, id);

            return true;
        }
        public virtual void HideIcon()
        {
            if (__icon != null)
                __icon.Dispose();
        }

        //
        //
        // Summary:
        //     Gets the interior of property.
        public virtual Interior Interior
        {
            get => __interior;
            set
            {
                if (value == null)
                {
                    foreach (Player p in __PlayersIn)
                        RemovePlayer(p);

                    if(__interior != null)
                    {
                        // Daca interiorul este null, anumal eventul
                        __interior.LeaveDoor -= __interior_LeaveExit;
                        __interior.TouchDoor -= __interior_TouchExit;
                    }
                    __interior = null;
                    Locked = true;
                }
                else
                {
                    // Ii atribuim noul interior inainte de inregistrarea evenimentelor.
                    __interior = value;

                    foreach (Player p in __PlayersIn)
                    {
                        p.Interior = __interior.GameInterior;
                        p.Position = __interior.Position;
                        p.Angle = __interior.Angle; 
                    }

                    // In caz de crash ne uitam aici
                    // Cand s-a aplicat un interior valid, inregistram evenimentul de iesire
                    __interior.LeaveDoor += __interior_LeaveExit;
                    __interior.TouchDoor += __interior_TouchExit;
                }
            }
        }

        //
        // Summary:
        //     Gets the angle of property.
        public virtual float Angle { get => __angle; set => __angle = value; }

        //
        // Summary:
        //     Determining if the property is locked.
        public virtual bool Locked
        {
            get => __locked;
            set
            {
                System.Console.WriteLine("Locked called from base");
                bool lk = value;

                if(!lk) // Open
                {
                    if (__interior == null)
                    {
                        lk = true;
                    }
                }
                __locked = lk;
            }
        }

        //
        // Summary:
        //     Gets a list of all players that are insinde the property.
        public List<Player> PlayersIn => __PlayersIn;

        //
        // Summary:
        //     Put player in property.
        public virtual bool PutPlayerIn(Player player)
        {
            if (player.Property == this)
                return false;

            if (__interior == null)
                return false;

            player.Interior = __interior.GameInterior; // Ii spun jucatorului interiorul din joc
            player.Position = __interior.Position; // Il trimit in interior
            player.Angle = __interior.Angle; // Ii ajustez privirea
            player.VirtualWorld = __pickup.Id; // HACK: folosesc id-ul pickup-ului pe post de virtual world
            player.Property = this; // Ii asum propietatea
            player.PropertyInteracting = this; // In acelasi timp interactioneaza cu propietatea, nu?
            __PlayersIn.Add(player); // Il adaug in lista de jucatori din propietate
            return true;
        }

        //
        // Summary:
        //     Evacuate the player from property.
        public virtual void RemovePlayer(Player player)
        {
            // Clean up
            player.Interior = 0;
            player.Position = __pos;
            player.Angle = __angle;
            player.VirtualWorld = 0;
            player.Property = null;
            player.PropertyInteracting = null;
            __PlayersIn.Remove(player);
        }

        //
        // Summary:
        //     Gets the money from the deposit of property.
        public virtual int Deposit { get => __deposit; set => __deposit = value; }
        public virtual int Price { get => __price; set => __price = value; }

        public virtual int Owner { get => __owner; set => __owner = value; }

        public virtual void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE properties SET interior=@interior, locked=@locked, deposit=@deposit, price=@price WHERE id=@id", conn);

                int inter = Interior.Index(Interior);
                if(inter == -1)
                    cmd.Parameters.AddWithValue("@interior", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@interior", inter);

                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.Parameters.AddWithValue("@deposit", Deposit);
                cmd.Parameters.AddWithValue("@price", Price);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }
        }

        public abstract void UpdateLabel();
        //public abstract int Id { get; }

        public abstract void SetOwnerUpdate(int id);

        private void __Spawn(PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            // Nu poate fii None, nu?
            if (type == PropertyType.TypeNone)
                type = PropertyType.TypeGeneric;

            // Buildup
            __type = type;
            __area = DynamicArea.CreateSphere(pos, 1.5f);
            __pickup = new DynamicPickup((int)type, 23, pos);
            __label = new DynamicTextLabel(ToString(), Color.White, pos, 30.0f);
            __label.TestLOS = true;
            __pos = pos;
            __angle = angle;
            __price = 0;
            Interior = interior;

            // Inregistram evenimentele
            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }
    }
}
