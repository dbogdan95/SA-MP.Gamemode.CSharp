using System.Timers;
using SampSharp.Streamer;
using SampSharp.GameMode;

namespace Game.World.Item
{
    public partial class Item
    {
        public static bool Drop(Player player)
        {
            System.Console.WriteLine("DropItem");
            if (player.HoldingItem is null)
                return false;

            //player.Lift = true;

            //if(__type.UseCarryAnim)
            //    player.ApplyAnimation("CARRY", "PUTDOWN", 5.0f, false, false, false, false, 2000, true);
            //else
            //    player.ApplyAnimation("BOMBER", "BOM_PLANT_IN", 5.0f, true, false, false, false, 450, true);

            //Timer t = new Timer();
            //t.Interval = 400;
            //t.Elapsed += ItemInteractTimer_Elapsed;
            //t.Start();

            //player.ItemInteractTimer = t;
            return true;
        }

        private void ItemInteractTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Player player = __holder;
            player.ApplyAnimation("BOMBER", "BOM_PLANT_2IDLE", 4.0f, false, false, false, false, 0, true);
            
            RemoveItemFromPlayer(player);
            Streamer.Update(player);

            player.Lift = false;
            player.ItemInteractTimer.Stop();
            player.ItemInteractTimer = null;
        }

        private bool __PickupItem(Player player)
        {
            if (!(player.HoldingItem is null))
                return false;

            System.Console.WriteLine("__PickupItem");

            //player.ClearAnimations(true);
            //player.Position = player.Position;
            //player.Angle = (float)Util.GetAngleToPoint(player.Position, Position);
            //player.Lift = true;

            //__interactor = player;

            //Timer t = new Timer(400);

            //if ((player.Position.Z - Position.Z) < .3)
            //{
            //    if(Type.UseCarryAnim)
            //        player.ApplyAnimation("CARRY", "LIFTUP105", 5.0f, false, false, false, false, 400, true);
            //    else
            //        player.ApplyAnimation("CASINO", "SLOT_PLYR", 5.0f, false, false, false, false, 0, true);

            //    t.Elapsed += __ItemPickupTimer_1;
            //}
            //else
            //{
            //    if (Type.UseCarryAnim)
            //        player.ApplyAnimation("CARRY", "LIFTUP", 5.0f, false, false, false, false, 400, true);
            //    else
            //        player.ApplyAnimation("BOMBER", "BOM_PLANT_IN", 5.0f, false, false, false, false, 450, true);

            //    t.Elapsed += __ItemPickupTimer_0;
            //}
            //t.Start();

            //player.ItemInteractTimer = t;
            return true;
        }

        private void __ItemPickupTimer_0(object sender, ElapsedEventArgs e)
        {
            __interactor.ApplyAnimation("BOMBER", "BOM_PLANT_2IDLE", 5.0f, false, false, false, false, 0, true);
            __ItemPickupTimer_1(sender, e);
        }

        private void __ItemPickupTimer_1(object sender, ElapsedEventArgs e)
        {
            Player player = __interactor;

            player.ItemInteractTimer.Stop();
            player.ItemInteractTimer = null;
            player.Lift = false;
            __interactor = null;

            GiveWorldItemToPlayer(player);
        }
    }
}
