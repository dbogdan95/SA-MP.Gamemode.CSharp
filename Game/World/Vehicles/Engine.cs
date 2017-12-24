using Game.World.Players;
using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Game.World.Vehicles
{
    public sealed class Engine
    {
        private readonly Timer __timer = new Timer(100);
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
            __timer.Elapsed += EngineTick;

            Console.WriteLine("Engine");
        }

        public void Switch(bool enable)
        {
            if (enable)
            {
                if (!__timer.Enabled)
                    __timer.Start();
            }
            else
            {
                if (!Vehicle.AnyEngineOn())
                    __timer.Stop();
            }
        }

        private void EngineTick(object sender, ElapsedEventArgs e)
        {
            IEnumerable<Vehicle> vehicles = Vehicle.GetAll<Vehicle>().Where(v => v.Engine);
            
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
