using Game.World;
using Game.World.Property;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;

namespace Game.Cmds
{
    class EditProperty
    {
        public void Begin(Player player, Property property)
        {
            ListDialog d = new ListDialog("Edit property", "Select", "Close");

            // From Pawn
            //format(str, 255, "- Level: %d\n- Interior: %d\n- World: %d\n- Price: %s\n- Rent & Fee: %s\n- Locked: %d\n- Label: %s\n- Owner: %s\n- Bizz Type: %s\n- Area: %d\n- Faction: %s\n- Gates: %d",

            d.AddItem("Interior: " + (property.Interior != null ? property.Interior.ToString() : "No interior"));
            d.AddItem("Locked: " + (property.Locked ? "yes" : "no"));
            d.AddItem("Deposit: " + Util.FormatNumber(property.Deposit));

            if (property is House)
            {
                House h = property as House;
                d.AddItem("Level: " + h.Level);
                d.AddItem("Rent: " + Util.FormatNumber(h.Rent));
            }

            d.Show(player);
            d.Response += (sender1, args1) =>
            {
                if (args1.DialogButton == DialogButton.Left)
                {
                    switch(args1.ListItem)
                    {
                        case 0:
                            {
                                ListDialog di = new ListDialog("Edit property - Interior", "Select", "Back");

                                // From Pawn
                                //format(str, 255, "- Level: %d\n- Interior: %d\n- World: %d\n- Price: %s\n- Rent & Fee: %s\n- Locked: %d\n- Label: %s\n- Owner: %s\n- Bizz Type: %s\n- Area: %d\n- Faction: %s\n- Gates: %d",

                                foreach(Interior interior in Interior.GetAll<Interior>())
                                {
                                    di.AddItem(interior.ToString());
                                }
                                di.Show(player);
                                di.Response += (sender2, args2) =>
                                {
                                    if (args2.DialogButton == DialogButton.Left)
                                    {
                                        property.Interior = Interior.GetAll<Interior>()[args2.ListItem];
                                        property.UpdateSql();

                                        d.Items[0] = "Interior: " + property.Interior.ToString();
                                    }
                                    d.Show(player);
                                };
                                break;
                            }
                        case 1:
                            {
                                property.Locked = !property.Locked;
                                property.UpdateSql();

                                d.Items[1] = "Locked: " + (property.Locked ? "yes" : "no");
                                d.Show(player);
                                break;
                            }
                        case 2:
                            {
                                break;
                            }
                        case 3:
                            {
                                if (property is House)
                                {
                                    InputDialog di = new InputDialog("Edit property - Level", "Change the level of house.", false, "Change", "Back");

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            if (int.TryParse(args2.InputText, out int n))
                                            {
                                                House h = property as House;
                                                h.Level = Math.Clamp(n, 1, 999);
                                                h.UpdateSql();

                                                d.Items[3] = "Level: " + h.Level;
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                break;
                            }
                        case 4:
                            {
                                if (property is House)
                                {
                                    InputDialog di = new InputDialog("Edit property - Rent", "Type rent of property(max $500).", false, "Change", "Back");

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if(args2.DialogButton == DialogButton.Left)
                                        {
                                            if (int.TryParse(args2.InputText, out int n))
                                            {
                                                House h = property as House;
                                                h.Rent = Math.Clamp(n, 0, 500);
                                                h.UpdateSql();

                                                d.Items[4] = "Rent: " + Util.FormatNumber(h.Rent);
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                break;
                            }
                    }
                }
            };
        }
    }

    [CommandGroup("prop")]
    class PropertiesAdmin
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
                        break;
                    }
                default:
                    {
                        sender.SendClientMessage("[ERROR] Invalid property type.");
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
                sender.SendClientMessage("[ERROR] There is no property around you.");
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
                sender.SendClientMessage("[ERROR] There is no property around you.");
                return;
            }

            EditProperty editProperty = new EditProperty();
            editProperty.Begin(player, property);
        }
    }
}
