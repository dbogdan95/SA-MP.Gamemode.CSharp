using SampSharp.GameMode.World;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using Game.Accounts;
using Game.World.Vehicles;
using Game.Display;
using Game.Factions;
using Game.World.Items;
using Game.World.Properties;
using SampSharp.GameMode.SAMP;

namespace Game.World.Players
{
    [PooledType]
    public partial class Player : BasePlayer
    {
        public bool IsLogged { get; set; }                  = false;
        public Account MyAccount { get; private set; }      = null;
        public Hud VehicleHud { get; private set; }         = null;
        public Item HoldingItem { get; set; }               = null;
        public bool Lift { get; set; }                      = false;
        public Timer ItemInteractTimer { get; set; }        = null;
        public Property Property { get; set; }              = null;
        public Property PropertyInteracting { get; set; }   = null;
        public House RentedRoom { get; set; }               = null;
        public House House { get; set; }                    = null;
        public Business Business { get; set; }              = null;
        public bool PropertyTranslation { get; set; }       = false;
        public bool PropertyDirection { get; set; }         = false;
        public MessageBox MessageBox { get; private set; }  = null;
        public FadeScreen FadeScreen { get; private set; }  = null;
        public Faction Faction { get; set; }                = null;
        public int? Rank { get; set; }                      = null;
        public bool SpawnAt { get; set; }                   = false;

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
