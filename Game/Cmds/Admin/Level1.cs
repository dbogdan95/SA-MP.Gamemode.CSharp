using Game.Cmds.ParameterTypes;
using Game.Factions;
using Game.World.Players;
using Game.World.Vehicles;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.Parameters;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;

namespace Game.Cmds.Admin
{
    class Level1
    {
        public class Level1PermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You need at least admin level 1."; }
            }

            public bool Check(BasePlayer player)
            {
                return true;
            }
            #endregion
        }

        [Command("finvite", PermissionChecker = typeof(Level1PermissionChecker), UsageMessage = "Usage: /finvite [playerid/PartOfName] [factionid/PartOfName]")]
        private static void CMD_InviteTo(Player sender, Player target, [Parameter(typeof(FactionType))]Faction faction)
        {
            try
            {
                faction.Invite(target);
                sender.SendClientMessage("** " + target.ToString() + " has been invited into " + faction.ToString());
                target.SendClientMessage("* Admin " + sender.Name + " just invited you into the faction " + faction.Name);
            }
            catch (Exception e)
            {
                sender.SendClientMessage("*** " + e.Message);
            }
        }

        [Command("fdismiss", PermissionChecker = typeof(Level1PermissionChecker), UsageMessage = "Usage: /fdismiss [playerid/PartOfName]")]
        private static void CMD_DismissFrom(Player sender, Player target)
        {
            try
            {
                target.Faction.Dismiss(target);
                sender.SendClientMessage("** " + target.ToString() + " has been dismissed from " + target.Faction.ToString());
                target.SendClientMessage("* Admin " + sender.Name + " just dismissed you from faction " + target.Faction.Name);
            }
            catch (Exception e)
            {
                sender.SendClientMessage("*** " + e.Message);
            }
        }

        [Command("changerank", PermissionChecker = typeof(Level1PermissionChecker), UsageMessage = "Usage: /changerank [playerid/PartOfName] [rankid (1-7)]")]
        private static void CMD_ChangeRank(Player sender, Player target, int rankid)
        {
            try
            {
                target.Faction.SetRank(target, rankid);
                sender.SendClientMessage("** " + target.ToString() + " has been changed to " + target.Faction.GetRanks[rankid].ToString());
                target.SendClientMessage("* Admin " + sender.Name + " just changed your rank to " + target.Faction.GetRanks[rankid].Name);
            }
            catch (Exception e)
            {
                sender.SendClientMessage("*** " + e.Message);
            }
        }

        [Command("saveacc", PermissionChecker = typeof(Level1PermissionChecker), UsageMessage = "Usage: /saveacc [playerid/PartOfName]")]
        private static void CMD_SaveAcc(Player sender, Player target)
        {
            if (!sender.IsLogged)
                return;

            target.MyAccount.Save();
            sender.SendClientMessage("** " + target.MyAccount.ToString() + " has been saved");
        }

        [Command("vehspawn", PermissionChecker = typeof(Level1PermissionChecker), UsageMessage = "Usage: /vehspawn [vehicleid/PartOfName]")]
        private static void CMD_VehSpawn(Player sender, VehicleModelType model)
        {
            if (!sender.IsLogged)
                return;

            Vehicle v = BaseVehicle.Create(model, sender.Position, 0.0f, -1, -1) as Vehicle;
            v.Fuel = Common.MAX_VEHICLE_FUEL;
            v.Engine = true;
            v.Lights = true;
        }
    }
}
