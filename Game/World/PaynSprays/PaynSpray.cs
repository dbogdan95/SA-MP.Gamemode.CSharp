using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.Streamer.World;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.World;
using Game.World.Players;
using Game.World.Vehicles;
using Game.World.Properties;

namespace Game.World.PaynSprays
{
    public partial class PaynSpray : StaticWorldObject<PaynSpray>
    {
        public int Model { get; private set; }
        public PaynSprayState State { get; private set; }
        public Vector3 OpenPosition { get; private set; }
        public Vector3 OpenRotation { get; private set; }
        public Vector3 ClosePosition { get; private set; }
        public Vector3 CloseRotation { get; private set; }
        public Vector3 AreaMins { get; private set; }
        public Vector3 AreaMaxs { get; private set; }
        public Vector3 Camera { get; private set; }

        #region Implementation of StaticWorldObject

        public override Vector3 Position
        {
            get
            {
                return new Vector3((AreaMins.X + AreaMaxs.X) / 2,
                    (AreaMins.Y + AreaMaxs.Y) / 2,
                    (AreaMins.Z + AreaMaxs.Z) / 2);
            }
        }

        #endregion

        private DynamicArea __zone;
        private DynamicObject __door;

        public event EventHandler<PaynSprayEventArgs> Repaired;

        public PaynSpray(int id, int model, Vector3 openPosition, Vector3 openRotation,
            Vector3 closePosition, Vector3 closeRotation, Vector3 areaMins, Vector3 areaMaxs, Vector3 camera) : base(id)
        {
            Model = model;
            OpenPosition = openPosition;
            OpenRotation = openRotation;
            ClosePosition = closePosition;
            CloseRotation = closeRotation;
            AreaMins = areaMins;
            AreaMaxs = areaMaxs;
            Camera = camera;

            __zone = DynamicArea.CreateCube(AreaMins, AreaMaxs);
            __door = new DynamicObject(Model, ClosePosition, CloseRotation);

            __door.Moved += __door_Moved;
            __zone.Enter += __zone_Enter;
            __zone.Leave += __zone_Leave;

            State = PaynSprayState.StateClosed;
        }

        public override string ToString()
        {
            return "Pay'n'Spray(" + Id + ")";
        }

        public int Close()
        {
            if (State == PaynSprayState.StateClosing || State == PaynSprayState.StateClosed)
                return 0;

            State = PaynSprayState.StateClosing;
            Util.PlaySoundInRangeOfPoint(Sounds.SOUND__CARGO_PLANE_DOOR_LOOP, OpenPosition, 25.0f);

            return __door.Move(ClosePosition, 1.0f, CloseRotation);
        }

        public int Open()
        {
            if (State == PaynSprayState.StateOpening || State == PaynSprayState.StateOpened)
                return 0;

            State = PaynSprayState.StateOpening;
            Util.PlaySoundInRangeOfPoint(Sounds.SOUND__CARGO_PLANE_DOOR_LOOP, OpenPosition, 25.0f);

            return __door.Move(OpenPosition, 1.0f, OpenRotation);
        }

        private void __door_Moved(object sender, EventArgs e)
        {
            State = (State == PaynSprayState.StateOpening) ? PaynSprayState.StateOpened : PaynSprayState.StateClosed;
            Util.PlaySoundInRangeOfPoint(Sounds.SOUND__CARGO_PLANE_DOOR_STOP, OpenPosition, 25.0f);
        }

        private void __zone_Enter(object sender, PlayerEventArgs e)
        {
            Player player = e.Player as Player;

            player.CameraPosition = Camera;
            player.SetCameraLookAt(player.Position);

            player.Update += __player_Update;
            player.Disconnected += __player_Disconnected;

            if (player.Vehicle is Vehicle vehicle && player.State == PlayerState.Driving)
            {
                int time = Close();

                vehicle.GetDamageStatus(out int panels, out int doors, out int lights, out int tires);
                float health = vehicle.Health;

                player.ToggleControllable(false);

                new Timer(time + 1500, false).Tick += (snd2, args2) =>
                {
                    Dialog t = new MessageDialog("Pay'n'Spray", FormatRepairReceipt(vehicle, out int totalCost, out int countReparations), "Repair", "Cancel");
                    
                    if(totalCost == 0)
                    {
                        Open();
                        player.ToggleControllable(true);
                        player.GameText("~n~~w~Your car looks good!~n~~w~Nothing to do here!", 4000, 3);
                        return;
                    }

                    t.Show(player);
                    t.Response += (snd1, args1) =>
                    {
                        if (args1.DialogButton == DialogButton.Left)
                        {
                            if (player.Money < totalCost)
                            {
                                vehicle.UpdateDamageStatus(panels, doors, lights, tires);
                                vehicle.Health = health;

                                Open();
                                player.ToggleControllable(true);
                                player.GameText("~n~~w~We repair, you pay!~n~" +
                                    "~w~We can't repair if you don't have money!", 4000, 3);
                            }
                            else
                            {
                                player.GiveMoney(-totalCost);
                                Repaired?.Invoke(this, new PaynSprayEventArgs(player, totalCost));

                                new Timer(300, true).Tick += (snd3, args3) =>
                                {
                                    if (countReparations <= 0)
                                    {
                                        Open();
                                        player.ToggleControllable(true);
                                        (snd3 as Timer).Dispose();
                                        return;
                                    }

                                    Util.PlaySoundInRangeOfPoint(Sounds.SOUND_BUY_CAR_MOD, player.Position, 25.0f);
                                    countReparations--;
                                };

                                Business b = Business.FindByDomain(Id, BusinessTypes.TypePaynSpray);
                                if (b != null)
                                {
                                    b.Deposit += (int)(totalCost - totalCost * Common.TAX_BUSINESS_PER_PRODUCTS);
                                    b.UpdateSql();
                                }
                            }
                        }
                        else
                        {
                            vehicle.UpdateDamageStatus(panels, doors, lights, tires);
                            vehicle.Health = health;

                            Open();
                            player.ToggleControllable(true);
                        }
                    };
                };
            }
            else
                e.Player.GameText("~w~What are you doing here?~n~~w~Shuu!", 3000, 3);
        }

        private void __player_Disconnected(object sender, DisconnectEventArgs e)
        {
            Player player = sender as Player;
            player.Vehicle?.Respawn();
        }

        private void __zone_Leave(object sender, PlayerEventArgs e)
        {
            e.Player.ToggleControllable(true);
            e.Player.PutCameraBehindPlayer();

            e.Player.Update -= __player_Update;
            e.Player.Disconnected -= __player_Disconnected;
        }

        private void __player_Update(object sender, PlayerUpdateEventArgs e)
        {
            Player player = sender as Player;
            player.CameraPosition = Camera;
            player.SetCameraLookAt(player.Position);
        }

        public static string FormatRepairReceipt(BaseVehicle vehicle, out int totalCost, out int countReparations)
        {
            vehicle.GetDamageStatus(out int panels, out int doors, out int lights, out int tires);
            Util.DecodePanels(panels, out int front_left_panel, out int front_right_panel, out int rear_left_panel, out int rear_right_panel, out int windshield, out int front_bumper, out int rear_bumper);
            Util.DecodeLights(lights, out int front_left_light, out int front_right_light, out int back_lights);
            Util.DecodeDoors(doors, out int bonnet, out int boot, out int driver_door, out int passager_door);
            Util.DecodeTires(tires, out int rear_right_tire, out int front_right_tire, out int rear_left_tire, out int front_left_tire);

            string receipt = "Your repair receipt: \n\n";
            int cost = 0, num;
            totalCost = 0;
            countReparations = 0;

            if (front_left_panel != 0 || front_right_panel != 0 || rear_left_panel != 0 || rear_right_panel != 0)
            {
                num = front_left_panel + front_right_panel + rear_left_panel + rear_right_panel;
                cost = Convert.ToInt32(PaynSprayPrices.Panels)*num;

                receipt += "Panels ("+ num + "x): " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += num;
            }
                
            if (windshield != 0)
            {
                cost = Convert.ToInt32(PaynSprayPrices.WindShild);
                receipt += "Windshield: " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += 1;
            }
            
            if (front_bumper != 0)
            {
                cost = Convert.ToInt32(PaynSprayPrices.Bumpers);
                receipt += "Front bumper: " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += 1;
            }
                
            if (rear_bumper != 0)
            {
                cost = Convert.ToInt32(PaynSprayPrices.Bumpers);
                receipt += "Rear bumper: " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += 1;
            }
             
            if (front_left_light != 0 || front_right_light != 0)
            {
                num = (front_left_light + front_right_light);
                cost = Convert.ToInt32(PaynSprayPrices.Lights) *num;
                receipt += "Headlights(" + num + "x): " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += num;
            }
              
            if (back_lights != 0)
            {
                cost = Convert.ToInt32(PaynSprayPrices.BackLights);
                receipt += "Backlights: " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += 1;
            }
             
            if(rear_right_tire != 0 || front_right_tire != 0 || rear_left_tire != 0 || front_left_tire != 0)
            {
                num = (rear_right_tire + front_right_tire + rear_left_tire + front_left_tire);
                cost = Convert.ToInt32(PaynSprayPrices.Wheels);

                receipt += "Wheels(" + num + "x): " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += num;
            }
            
            if(vehicle.Health < 620)
            {
                cost = Convert.ToInt32(PaynSprayPrices.Engine);
                receipt += "New engine: " + Util.FormatNumber(cost) + "\n";
                totalCost += cost;
                countReparations += 1;
            }

            receipt += "Total: " + Util.FormatNumber(totalCost);

            return receipt;
        }
    }
}
