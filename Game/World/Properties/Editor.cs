using Game.Accounts;
using Game.Factions;
using Game.World.Players;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Properties
{
    class Editor
    {
        public void Begin(Player player, Property property)
        {
            ListDialog d = new ListDialog("Edit property", "Select", "Close");

            // From Pawn
            //format(str, 255, "- Level: %d\n- Interior: %d\n- World: %d\n- Price: %s\n- Rent & Fee: %s\n- Locked: %d\n- Label: %s\n- Owner: %s\n- Bizz Type: %s\n- Area: %d\n- Faction: %s\n- Gates: %d",

            d.AddItem("Interior: " + property.Interior?.ToString());
            d.AddItem("Locked: " + (property.Locked ? "yes" : "no"));
            d.AddItem("Deposit: " + Util.FormatNumber(property.Deposit));

            if (property is House)
            {
                House h = property as House;
                d.AddItem("Level: " + h.Level);
                d.AddItem("Rent: " + Util.FormatNumber(h.Rent));
            }
            else if (property is Business)
            {
                Business b = property as Business;
                d.AddItem("Type: " + ((b.BizzType != null) ? b.BizzType.ToString() : "None"));
                d.AddItem("Linked to: " + b.Domainid);
            }
            else if (property is Generic)
            {
                d.AddItem("Faction: " + (property as Generic).Faction?.ToString());
            }

            if (!(property is Generic))
            {
                d.AddItem("Owner: " + Account.GetSQLNameFromSQLID(property.Owner));
                d.AddItem("Sell price: " + Util.FormatNumber(property.Price));
            }

            d.Show(player);
            d.Response += (sender1, args1) =>
            {
                if (args1.DialogButton == DialogButton.Left)
                {
                    switch (args1.ListItem)
                    {
                        case 0:
                            {
                                ListDialog di = new ListDialog("Edit property - Interior", "Select", "Back");

                                di.AddItem("No interior");

                                foreach (Interior interior in Interior.GetAll<Interior>())
                                {
                                    di.AddItem(interior.ToString());
                                }
                                di.Show(player);
                                di.Response += (sender2, args2) =>
                                {
                                    if (args2.DialogButton == DialogButton.Left)
                                    {
                                        if (args2.ListItem == 0)
                                            property.Interior = null;
                                        else
                                            property.Interior = Interior.FromIndex(args2.ListItem - 1);

                                        property.UpdateSql();

                                        d.Items[0] = "Interior: " + property.Interior?.ToString();
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
                                InputDialog di = new InputDialog("Edit property - Deposit", "Deposit or withdraw money from property. (use negative to withdraw)\n Note: Money will not enter into your account.", false, "Ok", "Back");

                                di.Show(player);
                                di.Response += (sender2, args2) =>
                                {
                                    if (args2.DialogButton == DialogButton.Left)
                                    {
                                        if (int.TryParse(args2.InputText, out int n))
                                        {
                                            property.Deposit += n;
                                            property.UpdateSql();

                                            d.Items[2] = "Deposit: " + Util.FormatNumber(property.Deposit);
                                        }
                                    }
                                    d.Show(player);
                                };
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
                                                h.UpdateLabel();

                                                d.Items[3] = "Level: " + h.Level;
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                else if (property is Business)
                                {
                                    ListDialog di = new ListDialog("Edit property - Business type", "Select", "Back");
                                    Dictionary<int, BusinessType> TypesDictionary = new Dictionary<int, BusinessType>();

                                    int i = 0;
                                    foreach (BusinessType type in BusinessType.AllTypes())
                                    {
                                        di.AddItem(type.ToString());
                                        TypesDictionary.Add(i, type);
                                        i++;
                                    }

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            Business b = property as Business;
                                            b.BizzType = TypesDictionary[args2.ListItem];
                                            b.UpdateSql();
                                            b.UpdateLabel();

                                            d.Items[3] = "Type: " + b.BizzType.ToString();
                                        }
                                        TypesDictionary = null;
                                        d.Show(player);
                                    };
                                }
                                else if (property is Generic)
                                {
                                    Dictionary<int, Faction> FactionsDictionary = new Dictionary<int, Faction>();
                                    ListDialog di = new ListDialog("Edit property - Faction", "Select", "Back");

                                    int i = 0;
                                    foreach (Faction faction in Faction.GetAll<Faction>())
                                    {
                                        di.AddItem(faction.ToString());
                                        FactionsDictionary.Add(i, faction);
                                        i++;
                                    }

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            Generic generic = property as Generic;
                                            generic.Faction = FactionsDictionary[args2.ListItem];
                                            generic.UpdateSql();

                                            d.Items[3] = "Faction: " + generic.Faction.ToString();
                                        }
                                        FactionsDictionary = null;
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
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            if (int.TryParse(args2.InputText, out int n))
                                            {
                                                House h = property as World.Properties.House;
                                                h.Rent = Math.Clamp(n, 0, 500);
                                                h.UpdateSql();
                                                h.UpdateLabel();

                                                d.Items[4] = "Rent: " + Util.FormatNumber(h.Rent);
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                else if (property is Business)
                                {
                                    InputDialog di = new InputDialog("Edit property - Link to", "Type the id of domain you want to link to this business.", false, "Set", "Back");

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            if (int.TryParse(args2.InputText, out int n))
                                            {
                                                Business b = property as Business;
                                                b.Domainid = n;
                                                b.UpdateSql();

                                                d.Items[4] = "Linked to: " + b.Domainid;
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                else
                                    d.Show(player);

                                break;
                            }
                        case 5:
                            {
                                player.SendClientMessage("cioaka");
                                if (!(property is Generic))
                                {
                                    InputDialog di = new InputDialog("Edit property - Owner", "Change the owner of property(0 to remove owner).", false, "Change", "Back");

                                    di.Show(player);
                                    di.Response += (sender2, args2) =>
                                    {
                                        if (args2.DialogButton == DialogButton.Left)
                                        {
                                            if (int.TryParse(args2.InputText, out int n))
                                            {
                                                if (!Account.IsSQLIDValid(n))
                                                    n = 0;

                                                property.SetOwnerUpdate(n);
                                                property.UpdateLabel();

                                                d.Items[5] = "Owner: " + Account.GetSQLNameFromSQLID(n);
                                            }
                                        }
                                        d.Show(player);
                                    };
                                }
                                else
                                    d.Show(player);
                                break;
                            }
                        case 6:
                            {
                                if (property.Type == PropertyType.TypeGeneric)
                                {
                                    d.Show(player);
                                    return;
                                }

                                InputDialog di = new InputDialog("Edit property - Sell price", "Change the sell price of property.", false, "Change", "Back");

                                di.Show(player);
                                di.Response += (sender2, args2) =>
                                {
                                    if (args2.DialogButton == DialogButton.Left)
                                    {
                                        if (int.TryParse(args2.InputText, out int n))
                                        {
                                            property.Price = n;
                                            property.UpdateSql();
                                            property.UpdateLabel();

                                            d.Items[6] = "Sell price: " + Util.FormatNumber(property.Price);
                                        }
                                    }
                                    d.Show(player);
                                };
                                break;
                            }
                    }
                }
            };
        }
    }
}
