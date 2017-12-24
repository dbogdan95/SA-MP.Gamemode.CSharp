using Game.Accounts;
using Game.World.Vehicles;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;

namespace Game.World.Players
{
    partial class Player
    {
        public override void OnSpawned(SpawnEventArgs e)
        {
            base.OnSpawned(e);
            PutCameraBehindPlayer();

            if(Faction != null)
            {
                Skin = Faction.Skin(Rank);
                Color = Faction.Color;
            }

            if (!MyAccount.LastPosition.IsEmpty)
            {
                PutInProperty(MyAccount.LastProperty);
                Position = MyAccount.LastPosition;
                Angle = MyAccount.LastAngle;

                MyAccount.ClearLastData();
            }
            else
            {
                if(SpawnAt)
                {
                    if (House != null)
                    {
                        if (PutInProperty(House))
                            return;
                    }
                    else if (RentedRoom != null)
                    {
                        if (PutInProperty(RentedRoom))
                            return;
                    }
                }

                if (Faction != null)
                {
                    if (!PutInProperty(Faction.Headquarter))
                        return;

                }

                Position = new Vector3(1023.1342, -1031.1912, 31.9978);
            }
        }

        public override void OnRequestClass(RequestClassEventArgs e)
        {
            SetSpawnInfo(NoTeam, 68, Vector3.Zero, 0.0f);

            if (IsLogged)
            {
                SendClientMessage("OnPlayerRequestClass");
                Spawn();
            }
            else
            {
                MyAccount = new Account(this);
                VehicleHud = new Vehicles.Hud(this);
                MessageBox = new Display.MessageBox(this);
                FadeScreen = new Display.FadeScreen(this);
            }

            base.OnRequestClass(e);
        }

        public override void OnStateChanged(StateEventArgs e)
        {
            if (e.NewState == PlayerState.Driving || e.NewState == PlayerState.Passenger)
            {
                Vehicle vehicle = Vehicle as Vehicle;

                if (vehicle.Faction != null && vehicle.Faction != Faction)
                {
                    SendClientMessage("*** You are a not member of " + vehicle.Faction.ToString() + ".");
                    RemoveFromVehicle();
                    return;
                }

                VehicleHud.Show();

                if (vehicle.Doors) VehicleHud.LockHud(true);
                if (vehicle.Lights) VehicleHud.LightHud(true);
            }

            if (e.OldState == PlayerState.Driving || e.OldState == PlayerState.Passenger)
            {
                if (VehicleHud != null)
                    VehicleHud.Hide();
            }

            base.OnStateChanged(e);
        }
    }
}
