using SampSharp.GameMode;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Linq;
using SampSharp.GameMode.Pools;
using MySql.Data.MySqlClient;
using Game.Core;
using Game.World.Players;

namespace Game.World.Properties
{
    public abstract partial class Property : IdentifiedPool<Property>
    {
        private PropertyType __type;
        private DynamicArea __area = null;
        private DynamicPickup __pickup = null;
        private DynamicTextLabel __label = null;
        private Interior __interior = null;
        private List<Player> __PlayersIn = new List<Player>();
        private Vector3 __pos;
        private float __angle;
        private bool __locked;
        private int __deposit;
        private int __price;
        private int? __owner;
        
        public Property(int id, PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            Id = id;
            __Spawn(type, interior, pos, angle);
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
            if (__type != PropertyType.TypeGeneric) __label.Dispose();
            //__area.Dispose();

            foreach (Player player in __PlayersIn)
                player.RemoveFromProperty();

            foreach (Player player in Player.GetAll<Player>().ToArray().Where(p => p.PropertyInteracting == this))
                player.PropertyInteracting = null;

            foreach (Player player in Player.GetAll<Player>().ToArray().Where(p => p.RentedRoom == this) )
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
                if (__type != PropertyType.TypeGeneric) __label.Position = new Vector3(__pos.X, __pos.Y, __pos.Z + 0.2);
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
        //     Gets the label of property.
        public virtual DynamicTextLabel Label => __label;

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

        public virtual int? Owner { get => __owner; set => __owner = value; }

        public virtual void UpdateSql()
        {
            Console.WriteLine("UpdateSql from base");

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

        public abstract void SetOwnerUpdate(int id);

        //public static void Load()
        //{
        //    int props = 0;
        //    using (var conn = Database.Connect())
        //    {
        //        MySqlCommand cmd = new MySqlCommand("SELECT A.*, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior, C.price, B.id AS owner " +
        //            "FROM houses as A " +
        //            "LEFT JOIN players as B ON A.id = B.house " +
        //            "INNER JOIN properties AS C ON c.id = A.baseProperty", conn);

        //        MySqlDataReader data = cmd.ExecuteReader();

        //        while (data.Read())
        //        {
        //            House h = new House(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"))
        //            {
        //                Locked = data.GetBoolean("locked"),
        //                Deposit = data.GetInt32("deposit"),
        //                Rent = data.GetInt32("rent"),
        //                Level = data.GetInt32("level"),
        //                Owner = data["owner"] is DBNull ? 0 : data.GetInt32("owner"),
        //                Price = data.GetInt32("price")
        //            };
        //            h.UpdateLabel();
        //            props++;
        //        }
        //        data.Close();

        //        cmd = new MySqlCommand("SELECT A.*, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior, C.price, B.id AS owner " +
        //            "FROM business as A " +
        //            "LEFT JOIN players as B ON A.id = B.business " +
        //            "INNER JOIN properties AS C ON c.id = A.baseProperty", conn);

        //        data = cmd.ExecuteReader();

        //        while (data.Read())
        //        {
        //            Business b = new Business(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"))
        //            {
        //                Locked = data.GetBoolean("locked"),
        //                Deposit = data.GetInt32("deposit"),
        //                BizzType = BusinessType.Find(data.GetInt32("type")),
        //                Domainid = data.GetInt32("domainid"),
        //                Owner = data["owner"] is DBNull ? 0 : data.GetInt32("owner"),
        //                Price = data.GetInt32("price")
        //            };
        //            b.UpdateLabel();
        //            props++;
        //        }
        //        data.Close();
        //    }
        //    Console.WriteLine("** Loaded {0} properties from database.", props);
        //}
    }
}
