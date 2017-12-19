using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;

namespace Game.World
{
    public class StaticWorldObject<T> : IdentifiedPool<T>, IWorldObject where T : StaticWorldObject<T>
    {
        public virtual Vector3 Position { get; set; }

        public StaticWorldObject(int id)
        {
            Id = id;
        }
    }
}
