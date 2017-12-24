using Game.World.Players;
using Game.World.Properties;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.SAMP.Commands.PermissionCheckers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Cmds.Admin
{
    class Level3
    {
        public class Level3PermissionChecker : IPermissionChecker
        {
            #region Implementation of IPermissionChecker

            public string Message
            {
                get { return "*** You need at least admin level 3."; }
            }

            public bool Check(BasePlayer player)
            {
                return true;
            }
            #endregion
        }

        [CommandGroup("prop", PermissionChecker = typeof(Level3PermissionChecker))]
        class PropertiesManipulation
        {
            [Command("spawn", UsageMessage = "Usage /prop [spawn] [type(1 = House, 2 = Business, 3 = Generic)]")]
            private static void SpawnCMD(BasePlayer sender, int type)
            {
                switch (type)
                {
                    case 1:
                        {
                            new House(null, sender.Position, sender.Angle);
                            break;
                        }
                    case 2:
                        {
                            new Business(null, sender.Position, sender.Angle);
                            break;
                        }
                    case 3:
                        {
                            new Generic(null, sender.Position, sender.Angle);
                            break;
                        }
                    default:
                        {
                            sender.SendClientMessage("*** Invalid property type.");
                            break;
                        }
                }
            }

            [Command("delete")]
            private static void DeleteCMD(BasePlayer sender)
            {
                Player player = (sender as Player);
                Property property = player.PropertyInteracting;

                if (property == null)
                {
                    sender.SendClientMessage("*** There is no property around you.");
                    return;
                }

                Dialog t = new MessageDialog("Delete?", "Are you sure you want to delete this property(irreversible)?", "Yes", "No");

                t.Show(player);
                t.Response += (snd, args) =>
                {
                    if (args.DialogButton == DialogButton.Left)
                    {
                        property.Remove();
                    }
                };
            }

            [Command("edit")]
            private static void EditCMD(BasePlayer sender)
            {
                Player player = (sender as Player);
                Property property = player.PropertyInteracting;

                if (property == null)
                {
                    sender.SendClientMessage("*** There is no property around you.");
                    return;
                }

                Editor editProperty = new Editor();
                editProperty.Begin(player, property);
            }
        }
    }
}
