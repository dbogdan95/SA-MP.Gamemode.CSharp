using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;

namespace Game.World.Property
{
    class House : Property
    {
        public House(Interior interior, Vector3 pos, float angle) : base(PropertyType.TypeHouse, pos, angle)
        {
            Interior = interior;
            Locked = false;
        }
    }
}
