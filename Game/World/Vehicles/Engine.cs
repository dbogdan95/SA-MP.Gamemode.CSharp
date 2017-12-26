using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.World.Vehicles
{
    public sealed class Engine
    {
        private readonly Timer __timer = new Timer(100, true);
        private static readonly Lazy<Engine> __instance = new Lazy<Engine>(()=>new Engine());

        public static Engine Instance
        {
            get
            {
                return __instance.Value;
            }
        }

        private Engine()
        {
            __timer.Tick += __timer_Tick;
        }

        public void Switch(bool enable)
        {
            if (enable)
            {
                if (!__timer.IsRunning)
                    __timer.IsRunning = true;
            }
            else
            {
                if (!Vehicle.AnyEngineOn())
                    __timer.IsRunning = false;
            }
        }

        private void __timer_Tick(object sender, EventArgs e)
        {
            IEnumerable<Vehicle> vehicles = Vehicle.GetAll<Vehicle>().ToArray().Where(v => v.Engine);
            
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Fuel -= 0.01f;

                if(vehicle.Fuel == 0)
                    vehicle.Engine = false;

                vehicle.UpdateHud();
            }
        }
    }
}
