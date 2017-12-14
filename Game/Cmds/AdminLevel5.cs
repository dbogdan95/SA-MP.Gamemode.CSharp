using Game.World.PaynSpray;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Cmds
{
    class AdminLevel5
    {
        [Command("dev")]
        private static void Dev(BasePlayer sender)
        {
            PaynSpray.ToggleDeveloper();
        }
    }
}
