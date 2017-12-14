using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World
{
    public class PaynSprayEventArgs : PlayerEventArgs
    {
        public PaynSprayEventArgs(BasePlayer player, int cost)
            : base(player)
        {
            Cost = cost;
        }

        public int Cost { get; private set; }
    }
}
