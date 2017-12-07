using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Controllers;
using SampSharp.Streamer.World;
using System.Collections.Generic;
using System;

namespace Game.World.Item
{
    public partial class ItemController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.PlayerKeyStateChanged += Item_OnPlayerKeyStateChange;
            gameMode.PlayerDied += Item_OnPlayerDied;
        }

        private void Item_OnPlayerDied(object sender, DeathEventArgs e)
        {
            Player player = sender as Player;
            if (player.HoldingItem != null)
            {
                player.HoldingItem.RemoveItem(player);
            }

            player.ItemInteractTimer.Dispose();
        }

        private void Item_OnPlayerKeyStateChange(object sender, KeyStateChangedEventArgs e)
        {
            Player player = sender as Player;

            if (player.Lift)
                return;

            if (player.State != PlayerState.OnFoot)
                return;

            if (player.InAnyVehicle)
                return;

            if (e.NewKeys == Keys.SecondaryAttack)
            {
                if (!player.ForceDropItem())
                {
                    // TODO
                    /*List<DynamicArea> list = DynamicArea.GetAreasForPlayer(player, 3);

                    Console.WriteLine(list.Count.ToString());

                    foreach (DynamicArea dynamicArea in list)
                    {
                        //if (!Item.ItemOfArea.ContainsKey(dynamicArea))
                        //    continue;

                        //Console.WriteLine(dynamicArea.ToString());

                        //Item.ItemOfArea[dynamicArea].ForcePlayerPickupItem(player);
                        //break;
                    }*/
                }
            }
        }
    }
}
