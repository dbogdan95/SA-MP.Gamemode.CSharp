using Game.World.Players;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Cmds
{
    class Casual
    {
        [Command("switchspawn", Shortcut = "spawnchange")]
        private static void CMD_SpawnSwitch(BasePlayer sender)
        {
            Player player = sender as Player;

            if (player.House == null)
            {
                player.SendClientMessage("* You don't have a house or rent.");
                return;
            }

            player.SpawnAt = !player.SpawnAt;

            if (player.SpawnAt)
            {
                player.SendClientMessage("* Now, you will be spawned at your house or rent.");
            }
            else
            {
                player.SendClientMessage("* Now, you will be spawned at your normal place.");
            }
        }
    }
}
