using System.Timers;
using Game.World.Item;
using Game.World.Property;
using Game.World.Property.Business;
using Game.World.Property.House;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Controllers;
using System;
using SampSharp.GameMode;

namespace Game
{
    public class Player : BasePlayer
    {
        public bool IsLogged { get; set; }
        public Account.Account MyAccount { get; set; }
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
            SetSpawnInfo(-1, 0, new Vector3(), 0.0f);
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

    public class PlayerController : ITypeProvider
    {
        public void RegisterTypes()
        {
            Player.Register<Player>();
        }
    }
}
