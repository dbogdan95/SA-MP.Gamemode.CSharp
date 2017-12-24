using Game.World.Players;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Properties
{
    class OwnerMenu
    {
        private Property __property;

        public OwnerMenu(Player player, Property property)
        {
            __property = property;

            ListDialog d = new ListDialog("My Property", "Select", "Close");

            d.AddItem("Deposit: " + Util.FormatNumber(property.Deposit));
            d.AddItem("Sell price: " + Util.FormatNumber(property.Price));

            if(property.Interior != null)
                d.AddItem("Locked: " + (property.Locked ? "{ef3434}yes" : "{33ef78}nop"));
            else
                d.AddItem("{d3d3d3}Locked: " + (property.Locked ? "yes" : "nop"));

            if (property is House)
                d.AddItem("Rent: " + Util.FormatNumber((property as House).Rent));

            d.Response += (sender, e) =>
            {
                if (e.DialogButton != DialogButton.Left)
                    return;

                switch (e.ListItem)
                {
                    case 0:
                        {
                            InputDialog di = new InputDialog("Property - Deposit", "Deposit or withdraw money from property. (use negative to withdraw)", false, "Ok", "Back");

                            di.Show(player);
                            di.Response += (sender2, args2) =>
                            {
                                if (args2.DialogButton == DialogButton.Left)
                                {
                                    if (int.TryParse(args2.InputText, out int n))
                                    {
                                        if (n > 0)
                                        {
                                            if (player.Money < n)
                                            {
                                                player.SendClientMessage("*** You don't have enough money in your account.");
                                            }
                                            else
                                            {
                                                player.Money -= n;
                                                __property.Deposit += n;
                                            }
                                        }
                                        else if (n < 0)
                                        {
                                            n = Math.Abs(n);

                                            if (__property.Deposit < n)
                                            {
                                                player.SendClientMessage("*** You don't have enough money in your deposit.");
                                            }
                                            else
                                            {
                                                player.Money += n;
                                                __property.Deposit -= n;
                                            }
                                        }

                                        d.Items[0] = "Deposit: " + Util.FormatNumber(property.Deposit);
                                        __property.UpdateSql();
                                    }
                                }
                                d.Show(player);
                            };
                            break;
                        }
                    case 1:
                        {
                            InputDialog di = new InputDialog("Property - Sell price", "Sell the property (0 means not for sell).", false, "Ok", "Back");

                            di.Show(player);
                            di.Response += (sender2, args2) =>
                            {
                                if (args2.DialogButton == DialogButton.Left)
                                {
                                    if (int.TryParse(args2.InputText, out int n))
                                    {
                                        __property.Price = Math.Max(n, 0);
                                        __property.UpdateSql();
                                        __property.UpdateLabel();

                                        d.Items[1] = "Sell price: " + Util.FormatNumber(property.Price);
                                    }
                                }
                                d.Show(player);
                            };
                            break;
                        }
                    case 2:
                        {
                            if (property.Interior != null)
                            {
                                __property.Locked = !__property.Locked;
                                __property.UpdateSql();

                                d.Items[2] = "Locked: " + (property.Locked ? "{ef3434}yes" : "{33ef78}nop");
                            }

                            d.Show(player);
                            break;
                        }
                    case 3:
                        {
                            InputDialog di = new InputDialog("House - Rent", "Set the rent price of the house (0 means not rentable).", false, "Ok", "Back");

                            di.Show(player);
                            di.Response += (sender2, args2) =>
                            {
                                if (args2.DialogButton == DialogButton.Left)
                                {
                                    if (int.TryParse(args2.InputText, out int n))
                                    {
                                        House house = __property as House;

                                        house.Rent = Math.Clamp(n, 0, 500);
                                        house.UpdateSql();
                                        house.UpdateLabel();

                                        d.Items[3] = "Rent: " + Util.FormatNumber(house.Rent);
                                    }
                                }
                                d.Show(player);
                            };
                            break;
                        }

                }
            };
            d.Show(player);
        }
    }
}
