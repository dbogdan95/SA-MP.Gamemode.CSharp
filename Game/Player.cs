using System.Timers;
using Game.World.Item;
using Game.World.Property;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Controllers;

namespace Game
{
    public class Player : BasePlayer
    {
        public Item HoldingItem { get; set; }
        public bool Lift { get; set; }
        public Timer ItemInteractTimer { get; set; }
        public Property Property { get; set; }
        public Property PropertyInteracting { get; set; }
        public House RentedRoom { get; set; }
        public House House { get; set; }

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

    public class PlayerController : ITypeProvider
    {
        public void RegisterTypes()
        {
            Player.Register<Player>();
        }
    }
}
