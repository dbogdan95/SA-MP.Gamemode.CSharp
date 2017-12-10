using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Property
{
    public abstract partial class Property : Pool<Property>
    {
        public Property(PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            // Nu poate fi None, nu?
            if (type == PropertyType.TypeNone)
                type = PropertyType.TypeGeneric;

            // Iconite doar pentru business-uri
            if (type == PropertyType.TypeBusiness)
                __icon = new DynamicMapIcon(pos, 0);

            // Buildup
            __type = type;
            __area = DynamicArea.CreateSphere(pos, 1.5f);
            __pickup = new DynamicPickup((int)type, 23, pos);
            __label = new DynamicTextLabel(ToString(), Color.White, pos, 30.0f);
            __pos = pos;
            __angle = angle;
            Interior = interior;

            // Inregistram evenimentele
            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }
        //
        // Summary:
        //     Dispose a property.
        public virtual void Remove()
        {
            // Curatam memoria
            //__pickup.Dispose();
            //__label.Dispose();
            __area.Dispose();

            // Doar business-urile au iconite
            if (__type == PropertyType.TypeBusiness)
                __icon.Dispose();

            foreach (Player player in __PlayersIn)
                player.RemoveFromProperty();

            foreach (Player player in Player.GetAll<Player>())
            {
                if (player.PropertyInteracting == this)
                    player.PropertyInteracting = null;
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
        public abstract void UpdateSql();
        public abstract void UpdateLabel();
    }
}
