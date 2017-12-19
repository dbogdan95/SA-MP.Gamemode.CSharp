using Game.World.Players;
using Game.World.Vehicles;
using SampSharp.GameMode.Display;
using System.Timers;

namespace Game.World.GasStations
{
    partial class GasStation
    {
        private class Refill
        {
            private GasStation __gasStation;
            private Player __player;
            private int __total;
            private int __gasUsed;
            private Timer __timer;

            internal Refill(GasStation gasStation, Player player)
            {
                __gasStation = gasStation;
                __player = player;
                __timer = new Timer(100);
                __total = 0;
                __gasUsed = 0;
            }

            internal void Begin()
            {
                __start();
            }

            internal void Stop()
            {
                __stop();
            }

            private void __start()
            {
                string price = Util.FormatNumber((int)GasStationPrice.PricePerLiter);

                __timer.Elapsed += (sender, e) =>
                {
                    Vehicle vehicle = (__player.Vehicle as Vehicle);

                    if (vehicle.Fuel >= Common.MAX_VEHICLE_FUEL)
                    {
                        vehicle.Fuel = Common.MAX_VEHICLE_FUEL;
                        __stop();

                        __player.SendClientMessage("** We hope you enjoyed our services.");
                        return;
                    }

                    if (__gasStation.Gas <= 0)
                    {
                        __gasStation.Gas = 0;
                        __stop();

                        __player.SendClientMessage("** We ran out of fuel, we will be back at next Payday.");
                        return;
                    }

                    if (__total >= __player.Money)
                    {
                        __stop();

                        __player.SendClientMessage("** That's it. You don't have enough money to continue the refill. " +
                            "No more freebies.");
                        return;
                    }

                    vehicle.Fuel++;
                    vehicle.UpdateHud();

                    __gasStation.Gas--;
                    __gasUsed++;
                    __total = (int)GasStationPrice.PricePerLiter * __gasUsed;

                    MessageDialog msg = new MessageDialog("Gas Station",
                        "* 1 liter = " + price + "\n" +
                        "* Gas used: " + __gasUsed + " liters\n" +
                        "-----------------------------\n" +
                        "* Current total: " + Util.FormatNumber(__total) + "\n" +
                        "Press 'stop' to stop the filling.", "stop");

                    msg.Response += (sender2, e2) =>
                    {
                        __player.SendClientMessage("** As you wish. Thanks you for using our services. Have a nice day!");
                        __player.Money -= __total;
                        __stop();
                    };
                    msg.Show(__player);
                };
                __timer.Start();
            }

            private void __stop()
            {
                __timer.Stop();
                __timer.Dispose();
                __player.Money -= __total;
                __gasStation.Pay(__total);

                Dialog.Hide(__player);
            }
        }
    }
}
