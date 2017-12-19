using Game.World.Players;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.GasStations
{
    partial class GasStation
    {
        private bool __on { get; set; }
        private DynamicArea __area;
        private float __gas;

        private Dictionary<Player, Refill> __customers = new Dictionary<Player, Refill>();

        private void __RegisterPlayer(Player player)
        {
            player.KeyStateChanged += Player_KeyStateChanged;
            player.Died += Player_Died;
            player.StateChanged += Player_StateChanged;
            player.Disconnected += Player_Disconnected;
        }

        private void __UnRegisterPlayer(Player player)
        {
            player.KeyStateChanged -= Player_KeyStateChanged;
            player.Died -= Player_Died;
            player.StateChanged -= Player_StateChanged;
            player.Disconnected -= Player_Disconnected;

            if (__customers.ContainsKey(player))
            {
                __customers[player].Stop();
                __customers.Remove(player);
            }
        }
    }
}
