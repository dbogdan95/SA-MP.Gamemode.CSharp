using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.Streamer.World;
using SampSharp.GameMode.SAMP;

namespace Game.World.Item
{
    public partial class Item
    {
        private void CreateInWorld(Vector3 pos = new Vector3(), Vector3 rot = new Vector3(), int world = -1, int interior = -1, bool applyrotoffsets = false)
        {
            __pos = pos;
            __rot = rot;
            __world = world;
            __interior = interior;

            if (__holder != null)
            {
                __holder.RemoveAttachedObject((int)Common.Attachments.AttachIndexItem);
                __holder.SpecialAction = SpecialAction.None;
                __holder = null;
            }

            __interactor = null;

            pos += new Vector3(0.0f, 0.0f, __type.Zoffset);

            if (applyrotoffsets)
            {
                __obj = new DynamicObject
                (
                    __type.Model,
                    pos,
                    rot.IsEmpty ? __type.DefaultRot : (__rot + __type.DefaultRot),
                    __world,
                    __interior
                );
            }
            else
            {
                __obj = new DynamicObject
                (
                    __type.Model,
                    pos,
                    rot.IsEmpty ? __type.DefaultRot : __rot,
                    __world,
                    __interior
                );
            }

            __area = DynamicArea.CreateSphere(pos, 1.5f, __world, __interior);
            ItemOfArea.Add(__area, this);

            //__area.Enter += Item_OnTouch;

            ItemsInWorld.Add(this);
        }

        private bool GiveWorldItemToPlayer(Player player)
        {
            if(!IsInWorld())
                return false;
            
            __holder = player;
            player.HoldingItem = this;
            ItemOfArea.Remove(__area);

            player.SetAttachedObject((int)Common.Attachments.AttachIndexItem, __type.Model, __type.AttachBone, __type.AttachPos, __type.AttachRot, __type.Scale, Color.White, Color.White);
            
            if (__type.UseCarryAnim)
                player.SpecialAction = SpecialAction.Carry;

            ItemsInWorld.Remove(this);

            __obj.Dispose();
            __area.Dispose();
            return true;
        }

        private Item RemoveItemFromPlayer(Player player)
        {
            Item item = player.HoldingItem;

            if (item is null)
                return null;

            player.RemoveAttachedObject((int)Common.Attachments.AttachIndexItem);
            player.SpecialAction = SpecialAction.None;

            player.HoldingItem = null;

            __holder = null;
            __interactor = null;

            CreateInWorld
            (
                player.Position + new Vector3(0.5 * Math.Sin(-player.Angle * Math.PI / 180), 0.5 * Math.Cos(-player.Angle * Math.PI / 180), -0.8), 
                new Vector3(), 
                player.VirtualWorld, 
                player.Interior, 
                true
            );

            return item;
        }
    }
}
