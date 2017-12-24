using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Cmds.Admin
{
    class Level2
    {
        public class Level2PermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You need at least admin level 2."; }
            }

            public bool Check(BasePlayer player)
            {
                return true;
            }
            #endregion
        }
    }
}
