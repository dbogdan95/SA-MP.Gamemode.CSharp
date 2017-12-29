using Game.Factions;
using Game.World.Players;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System.Linq;

namespace Game.Cmds
{
    class Factions
    {
        public class NonDepartamentPermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You are not in a suitable faction."; }
            }

            public bool Check(BasePlayer sender)
            {
                Player player = (sender as Player);

                if (player.Faction == null)
                    return false;

                if (FactionCategory.CategoryPolice == player.Faction.Category
                    || FactionCategory.CategoryFBI == player.Faction.Category
                    || FactionCategory.CategoryArmy == player.Faction.Category
                    || FactionCategory.CategoryeMedics == player.Faction.Category)
                    return false;

                return true;
            }
            #endregion
        }
        public class DepartamentPermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You are not in a suitable faction."; }
            }

            public bool Check(BasePlayer sender)
            {
                Player player = (sender as Player);

                if (player.Faction == null)
                    return false;

                if (FactionCategory.CategoryPolice != player.Faction.Category
                    && FactionCategory.CategoryFBI != player.Faction.Category
                    && FactionCategory.CategoryArmy != player.Faction.Category
                    && FactionCategory.CategoryeMedics == player.Faction.Category)
                    return false;

                return true;
            }
            #endregion
        }

        [Command("f", PermissionChecker = typeof(NonDepartamentPermissionChecker))]
        private static void CMD_FChat(Player sender, string message)
        {
            Player player = sender as Player;
            player.Faction.SendMessage(Color.RosyBrown, "<< " + player.Name + ": " + message + ">>");
        }

        [Command("d", PermissionChecker = typeof(DepartamentPermissionChecker))]
        private static void CMD_DChat(Player sender, string message)
        {
            Player player = sender as Player;
            string toSend = "<*> " +
                player.Faction.Name + "(" +
                player.Faction.GetRanks[player.Rank.Value].Name + ")" + " - " +
                player.Name + ": " + message + "<*>";

            foreach (Faction faction in Faction.GetAll<Faction>().Where(fac => FactionCategory.CategoryPolice == fac.Category
                    || FactionCategory.CategoryFBI == fac.Category
                    || FactionCategory.CategoryArmy == fac.Category
                    || FactionCategory.CategoryeMedics == fac.Category))
            {
                faction.SendMessage(Color.RosyBrown, toSend);
            }
        }

        [Command("r", PermissionChecker = typeof(DepartamentPermissionChecker))]
        private static void CMD_RChat(Player sender, string message)
        {
            Player player = sender as Player;
            player.Faction.SendMessage(player.Faction.Color, "* " + player.Name + " (radio): " + message + " over.");
        }
    }
}
