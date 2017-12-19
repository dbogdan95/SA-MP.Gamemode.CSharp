using System.Timers;
using Game.World.Items;
using Game.World.Properties;
using SampSharp.GameMode.World;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using Game.Accounts;
using Game.World.Vehicles;

namespace Game.World.Players
{
    [PooledType]
    public partial class Player : BasePlayer
    {
        public bool IsLogged { get; set; }
        public Account MyAccount { get; private set; }
        public VehicleHud VehicleHud { get; private set; }
        public Item HoldingItem { get; set; }
        public bool Lift { get; set; }
        public Timer ItemInteractTimer { get; set; }
        public Property Property { get; set; }
        public Property PropertyInteracting { get; set; }
        public House RentedRoom { get; set; }
        public House House { get; set; }
        public Business Business { get; set; }
        public bool PropertyTranslation { get; set; }
        public bool PropertyDirection { get; set; }
        
        public override void Spawn()
        {
            SetSpawnInfo(-1, 0, Vector3.Zero, 0.0f);
            base.Spawn();
        }

        public bool PutInProperty(Property property)
        {
            if (property == null)
                return false;

            if(property.Interior == null)
            {
                Position = property.Position;
                return true;
            }

            return property.PutPlayerIn(this);
        }

        public Property RemoveFromProperty()
        {
            if (Property == null)
            {
                return null;
            }

            Property.RemovePlayer(this);
            return Property;
        }

        public bool ForceDropItem()
        {
            System.Console.WriteLine("ForceDropItem");

            if (HoldingItem == null)
            {
                return false;
            }

            return Item.Drop(this);
        }
    }
}
