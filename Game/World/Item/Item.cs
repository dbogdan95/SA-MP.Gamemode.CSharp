using System;
using System.Collections.Generic;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.Streamer.World;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;

namespace Game.World.Item
{
    public partial class Item : Pool<Item>
    {
        public static List<Item> ItemsInWorld = new List<Item>();
        public static Dictionary<DynamicArea, Item> ItemOfArea = new Dictionary<DynamicArea, Item>();

        private ItemType        __type;
        private DynamicObject   __obj;
        private DynamicArea     __area;
        private Vector3         __pos;
        private Vector3         __rot;
        private int             __world;
        private int             __interior;
        private string          __exName;
        private Player          __holder;
        private Player          __interactor;

        public Item(ItemType itemType)
        {
            __type = itemType;
            __exName = String.Empty;

            CreateInWorld();
        }

        public Item(ItemType itemType, Vector3 pos)
        {
            __type = itemType;
            __exName = String.Empty;

            CreateInWorld(pos);
        }

        public Item(ItemType itemType, Vector3 pos, Vector3 rot)
        {
            __type = itemType;
            __exName = String.Empty;

            CreateInWorld(pos, rot);
        }

        public Item(ItemType itemType, Vector3 pos, Vector3 rot, int world)
        {
            __type = itemType;
            __exName = String.Empty;

            CreateInWorld(pos, rot, world);
        }

        public Item(ItemType itemType, Vector3 pos, Vector3 rot, int world, int interior)
        {
            __type = itemType;
            __exName = String.Empty;
            
            CreateInWorld(pos, rot, world, interior);
        }

        ~Item()
        {
            __area.Dispose();
            __obj.Dispose();
        }

        public ItemType Type { get => __type; }
        public DynamicObject Object { get => __obj; }
        public DynamicArea Area { get => __area; }
        public Vector3 Position { get => __pos; set => __area.Position = __obj.Position = __pos = value;  }
        public Vector3 Rotation { get => __rot; set => __obj.Rotation = __rot = value; }
        public int World { get => __world; set => __world = value; }
        public int Interior { get => __interior; set => __interior = value; }
        public string Name { get => __type.Name + __exName ?? " (" + __exName + ")"; }
        public string ExName { get => __exName; set => __exName = value; }
        public Player Holder { get => __holder; set => __holder = value; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (IsInWorld())
            {
                ItemsInWorld.Remove(this);

                __area.Dispose();
                __obj.Dispose();
            }
            else
            {
                __holder.RemoveAttachedObject((int)Common.Attachments.AttachIndexItem);
                __holder.HoldingItem = null;
            }
        }

        public bool IsInWorld()
        {
            return ItemsInWorld.Contains(this);
        }

        public bool Give(Player player)
        {
            return GiveWorldItemToPlayer(player);
        }

        public Item RemoveItem(Player player)
        {
            return RemoveItemFromPlayer(player);
        }

        public bool ForcePlayerPickupItem(Player player)
        {
            return __PickupItem(player);
        }
    }
}
