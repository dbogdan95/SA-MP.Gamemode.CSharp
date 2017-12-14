using SampSharp.GameMode;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Property
{
    partial class Property
    {
        private void __togglePlayer(Player player, bool In)
        {
            if (player.Lift || player.State != SampSharp.GameMode.Definitions.PlayerState.OnFoot)
                return;

            if (In)
                PutPlayerIn(player);
            else
                player.RemoveFromProperty();
        }
    }
}
