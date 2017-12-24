using Game.World.PaynSprays;
using Game.World.Players;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;

namespace Game.Cmds
{
    class Level5
    {
        public class Level5PermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You need at least admin level 5."; }
            }

            public bool Check(BasePlayer player)
            {
                return true;
            }
            #endregion
        }

        [Command("dev", PermissionChecker = typeof(Level5PermissionChecker))]
        private static void Dev(BasePlayer sender)
        {
            PaynSpray.ToggleDeveloper();
        }
    }
}
