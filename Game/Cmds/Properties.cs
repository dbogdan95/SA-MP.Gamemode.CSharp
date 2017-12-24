using Game.World.Properties;
using Game.World.Players;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;

namespace Game.Cmds
{
    class Properties
    {
        public class PropAroundPermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            /// <summary>
            ///     Gets the message displayed when the player is denied permission.
            /// </summary>
            public string Message
            {
                get { return "*** There is no property around you."; }
            }

            /// <summary>
            ///     Checks the permission for the specified player.
            /// </summary>
            /// <param name="player">The player.</param>
            /// <returns>true if allowed; false if denied.</returns>
            public bool Check(BasePlayer sender)
            {
                return (sender as Player).PropertyInteracting != null;
            }
            #endregion
        }
        public class HouseAroundPermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            /// <summary>
            ///     Gets the message displayed when the player is denied permission.
            /// </summary>
            public string Message
            {
                get { return "*** There is no property around you."; }
            }

            /// <summary>
            ///     Checks the permission for the specified player.
            /// </summary>
            /// <param name="player">The player.</param>
            /// <returns>true if allowed; false if denied.</returns>
            public bool Check(BasePlayer sender)
            {
                return (sender as Player).PropertyInteracting is World.Properties.House;
            }
            #endregion
        }

        [Command("property", PermissionChecker = typeof(PropAroundPermissionChecker))]
        private static void CMD_Menu(BasePlayer sender)
        {
            Player player = (sender as Player);
            Property property = player.PropertyInteracting;

            if(player.House != property && player.Business != property)
            {
                sender.SendClientMessage("*** This is not your personal property. Don't be a thief!");
                return;
            }

            OwnerMenu ownerMenu = new OwnerMenu(player, property); 
        }
    }
}
