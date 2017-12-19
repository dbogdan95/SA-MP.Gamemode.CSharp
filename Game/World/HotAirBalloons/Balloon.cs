using SampSharp.GameMode;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.HotAirBalloons
{
    class Balloon
    {
        static Balloon()
        {
            BaseMode.Instance.Initialized += (sender, e) =>
            {
                Console.WriteLine("Balloon()");
                new DynamicObject(19336, new Vector3(-2226.35059, -1741.96802, 479.98038), new Vector3(0.00000, 0.00000, -98.03998));
            };
        }
    }
}
