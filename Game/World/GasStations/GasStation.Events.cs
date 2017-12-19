using Game.World.Players;
using Game.World.Vehicles;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.GasStations
{
    partial class GasStation
    {
        private void __area_Leave(object sender, PlayerEventArgs e)
        {
            LeaveGasStation?.Invoke(this, new PlayerEventArgs(e.Player));
            __UnRegisterPlayer(e.Player as Player);
        }

        private void __area_Enter(object sender, PlayerEventArgs e)
        {
            EnterGasStation?.Invoke(this, new PlayerEventArgs(e.Player));

            if (__on)
            {
                if (Gas > 0)
                {
                    __RegisterPlayer(e.Player as Player);
                    e.Player.SendClientMessage("** Use LALT to start refill.");
                }
                else
                    e.Player.SendClientMessage("** Sadly, we ran out of fuel. We will be back at next Payday.");
            }
            else
                e.Player.SendClientMessage("** Sorry, we are offline today.");
        }

        private void Player_Died(object sender, DeathEventArgs e)
        {
            __UnRegisterPlayer(sender as Player);
        }
        
        private void Player_Disconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            __UnRegisterPlayer(sender as Player);
        }
        
        private void Player_StateChanged(object sender, StateEventArgs e)
        {
            if(e.OldState == SampSharp.GameMode.Definitions.PlayerState.Driving)
            {
                __UnRegisterPlayer(sender as Player);
            }
        }

        private void Player_KeyStateChanged(object sender, KeyStateChangedEventArgs e)
        {
            Player player = sender as Player;

            if (e.NewKeys == SampSharp.GameMode.Definitions.Keys.Fire) // LALT ON VEHICLE
            {
                if (player.Vehicle is Vehicle vehicle)
                {
                    if (vehicle.Engine)
                    {
                        player.SendClientMessage("** We cannot refill your vehicle if the engine is on.");
                        return;
                    }

                    if (Gas > 0)
                    {
                        if (vehicle.Fuel == Common.MAX_VEHICLE_FUEL)
                            return;

                        MessageDialog dialog = new MessageDialog("Gas Station", "Cost per liter: " + Util.FormatNumber((int)GasStationPrice.PricePerLiter) +
                            "Press start to begin the refill.", "start", "cancel");

                        dialog.Response += (sender2, e2) =>
                        {
                            if (e2.DialogButton == SampSharp.GameMode.Definitions.DialogButton.Right)
                                return;

                            __customers.Add(player, new Refill(this, player));
                            __customers[player].Begin();
                        };
                        dialog.Show(player);
                    }
                }
            }
        }
    }
}
