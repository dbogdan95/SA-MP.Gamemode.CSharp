using Game.World.PaynSprays;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;

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
