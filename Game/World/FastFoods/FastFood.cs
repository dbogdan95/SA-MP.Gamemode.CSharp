using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.FastFoods
{
    public abstract class FastFood : StaticWorldObject<FastFood>
    {
        private Marker _marker;

        public FastFood(int id, Vector3 pos) : base(id)
        {
            _marker = new Marker(pos);
            Position = pos;
        }
    }
}
