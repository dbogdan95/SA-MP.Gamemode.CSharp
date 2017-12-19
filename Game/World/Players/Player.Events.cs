using Game.Accounts;
using Game.World.Vehicles;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Players
{
    partial class Player
    {
        public override void OnSpawned(SpawnEventArgs e)
        {
            PutCameraBehindPlayer();
            base.OnSpawned(e);
        }

        public override void OnRequestClass(RequestClassEventArgs e)
        {
            SetSpawnInfo(Player.NoTeam, 68, Vector3.Zero, 0.0f);

            if (IsLogged)
            {
                SendClientMessage("OnPlayerRequestClass");
                Spawn();
            }
            else
            {
                MyAccount = new Account(this);
                VehicleHud = new VehicleHud(this);
            }

            base.OnRequestClass(e);
        }

        public override void OnStateChanged(StateEventArgs e)
        {
            if (e.NewState == PlayerState.Driving || e.NewState == PlayerState.Passenger)
            {
                Vehicle vehicle = Vehicle as Vehicle;
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
