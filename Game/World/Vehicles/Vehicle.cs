using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using System;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Controllers;
using Game.World.Players;
using System.Linq;
using System.Collections.Generic;

namespace Game.World.Vehicles
{
    [PooledType]
    public partial class Vehicle : BaseVehicle
    {
        private float __fuel;

        public float Fuel { get => __fuel; set => __fuel = Math.Clamp(value, 0, Common.MAX_VEHICLE_FUEL); }

        public Vector3 PostionFromOffset(Vector3 offset)
        {
            Vector3 v = Position;
            float r = Angle * Convert.ToSingle(Math.PI / 180);

            return new Vector3((Math.Sin(r) * offset.Y + Math.Cos(r) * offset.X + v.X), (Math.Cos(r) * offset.Y - Math.Sin(r) * offset.X + v.Y), (offset.Z + v.Z));
        }

        public override bool Doors
        {
            get => base.Doors;
            set
            {
                IEnumerable<Player> players = AllPasaggers();

                foreach (Player p in players)
                    p.VehicleHud.LockHud(value);

                base.Doors = value;
            }
        }

        public override bool Lights
        {
            get => base.Lights;
            set
            {
                IEnumerable<Player> players = AllPasaggers();

                foreach (Player p in players)
                    p.VehicleHud.LightHud(value);

                base.Lights = value;
            }
        }

        public override bool Engine
        {
            get => base.Engine;
            set
            {
                if (Fuel == 0)
                    base.Engine = false;
                else
                    base.Engine = value;

                if (base.Engine == false)
                {
                    if (!IsAnyVehicleWithEngineOn())
                        EngineTimer.Stop();
                }
                else
                {
                    if (!EngineTimer.Enabled)
                        EngineTimer.Start();
                }
            }
        }

        public void UpdateHud()
        {
            Vector3 vel = Velocity;
            IEnumerable<Player> passengers = AllPasaggers();
            if (passengers.Count() > 0)
            {
                double speed = Math.Sqrt(vel.X * vel.X + vel.Y * vel.Y + vel.Z * vel.Z) * 180;

                foreach (Player player in passengers)
                {
                    player.VehicleHud.SpeedoValue(speed);
                    player.VehicleHud.FuelValue(Fuel, Common.MAX_VEHICLE_FUEL);
                }
            }
        }

        IEnumerable<Player> AllPasaggers()
        {
            return Player.GetAll<Player>().Where(p => p.Vehicle == this);
        }
    }
}
