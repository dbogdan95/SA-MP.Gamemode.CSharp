using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;

namespace Game.Display
{
    public class FadeScreenEventArgs : PlayerEventArgs
    {
        public FadeScreenEventArgs(BasePlayer player, FadeScreenMode mode) : base(player)
        {
            Mode = mode;
        }

        public FadeScreenMode Mode { get; }
    }
}