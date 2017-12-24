using SampSharp.GameMode;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.HotAirBalloons
{
    class Balloon
    {
        static List<Vector3> __flyNodes = new List<Vector3>
        {
            new Vector3(-2226.35059, -1741.96802, 479.98038),
            new Vector3(-614.7222, -1308.9083, 405.0372),
            new Vector3(1544.1813, -1353.5040, 373.0132)
        };
        
        static Balloon()
        {
            BaseMode.Instance.Initialized += (sender, e) =>
            {
                Console.WriteLine("Balloon()");
                new DynamicObject(19336, new Vector3(-2226.35059, -1741.96802, 479.98038), new Vector3(0.00000, 0.00000, -98.03998));
            };
        }

        private uint __node = 0;
    }
}
