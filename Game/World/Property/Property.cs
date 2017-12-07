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
        public Property(PropertyType type, Vector3 pos, float angle)
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
            __interior = null;
            __pos = pos;
            __angle = angle;
            __locked = false;

            // Inregistram evenimentele
            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }
        
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
                if(__type == PropertyType.TypeBusiness)
                    __icon.Position = __pos;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            // Curatam memoria
            __pickup.Dispose();
            __label.Dispose();
            __area.Dispose();

            // Doar business-urile au iconite
            if (__type == PropertyType.TypeBusiness)
                __icon.Dispose();
        }

        public override string ToString()
        {
            return __type.ToString().Remove(0, 4);
        }

        public virtual PropertyType Type => __type;
        public virtual DynamicPickup Pickup => __pickup;
        public virtual DynamicMapIcon Icon => __icon;
        public virtual DynamicTextLabel Label => __label;
        public virtual Interior Interior
        {
            get => __interior;
            set
            {
                __interior = value;

                if (__interior == null)
                {
                    foreach (Player p in __PlayersIn)
                        RemovePlayer(p);

                    // Daca interiorul este null, anumal eventul
                    __interior.LeaveDoor -= __interior_LeaveExit;
                    __interior.TouchDoor -= __interior_TouchExit;
                }
                else
                {
                    // In caz de crash ne uitam aici
                    // Cand s-a aplicat un interior valid, inregistram evenimentul de iesire
                    __interior.LeaveDoor += __interior_LeaveExit;
                    __interior.TouchDoor += __interior_TouchExit;
                }
            }
        }

        public virtual float Angle { get => __angle; set => __angle = value; }
        public virtual bool Locked { get => __locked; set => __locked = value; }
        public List<Player> PlayersIn => __PlayersIn;

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
            __PlayersIn.Add(player); // Il adaug in lista de jucatori din propietate
            return true;
        }

        public virtual void RemovePlayer(Player player)
        {
            // Clean up
            player.Interior = 0;
            player.Position = __pos;
            player.Angle = __angle;
            player.VirtualWorld = 0;
            player.Property = null;
            __PlayersIn.Remove(player);
        }
    }
}
