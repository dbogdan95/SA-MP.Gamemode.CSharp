using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.GasStation
{
    class GasStation : IdentifiedPool<GasStation>
    {
        public Vector3 AreaMins { get; private set; }
        public Vector3 AreaMaxs { get; private set; }
    }
}
