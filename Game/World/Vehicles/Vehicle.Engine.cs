using Game.World.Players;
using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Game.World.Vehicles
{
    partial class Vehicle
    {
        static Timer EngineTimer = new Timer(100);

        static Vehicle()
        {
            EngineTimer.Elapsed += EngineTimer_Elapsed;
        }

        static bool IsAnyVehicleWithEngineOn()
        {
            return (GetAll<Vehicle>().Where(v => v.Engine).Count() > 0);
        }

        private static void EngineTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            IEnumerable<Vehicle> vehicles = GetAll<Vehicle>().Where(v => v.Engine);

            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Fuel -= 0.01f;

                if(vehicle.Fuel == 0)
                    vehicle.Engine = false;

                Console.WriteLine("EngineTimer_Elapsed " + vehicle.Fuel);
                vehicle.UpdateHud();
            }
        }
    }
}
