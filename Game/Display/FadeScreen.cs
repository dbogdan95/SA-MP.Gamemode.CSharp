using Game.World.Players;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;

namespace Game.Display
{
    public class FadeScreen
    {
        static int FRAMES = 100;
        static byte MAX_TRANSPARENCY = 0xFF;

        public event EventHandler<FadeScreenEventArgs> ScreenFadeEnd;

        public FadeScreenMode Mode { get => __mode; }
        public Color Color { get => __color; }

        Player __player;
        byte __alpha;
        Color __color;
        int __tpf;
        FadeScreenMode __mode;
        PlayerTextDraw __tdx;
        Timer __timer;

        bool __CompleteModeSwicher;

        public FadeScreen(Player player)
        {
            __player = player;
            __timer = new Timer(FRAMES, true)
            {
                IsRunning = false
            };
            __timer.Tick += __timer_Elapsed;
            __tdx = new PlayerTextDraw(__player)
            {
                Position = new Vector2(-20.0, -20.0),
                Font = SampSharp.GameMode.Definitions.TextDrawFont.Pricedown,
                LetterSize = new Vector2(680.0, 500.0),
                UseBox = true
            };

            player.Disconnected += Player_Disconnected;
        }

        public void Start(int ms)
        {
            __FadeScreen(ms, FadeScreenMode.ModeFadeIn, Color.Black);
        }

        public void Start(int ms, FadeScreenMode mode)
        {
            __FadeScreen(ms, mode, Color.Black);
        }

        public void Start(int ms, FadeScreenMode mode, Color tocolor)
        {
            __FadeScreen(ms, mode, tocolor);
        }

        public bool IsFading => __timer.IsRunning;

        private void __timer_Elapsed(object sender, EventArgs e)
        {
            switch(__mode)
            {
                case FadeScreenMode.ModeFadeIn:
                    {
                        __alpha = MAX(__alpha + __tpf);

                        if(__alpha == MAX_TRANSPARENCY)
                        {
                            ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeFadeIn));

                            __timer.IsRunning = false;
                        }
                        break;
                    }
                case FadeScreenMode.ModeFadeOut:
                    {
                        __alpha = MIN(__alpha - __tpf);

                        if (__alpha == 0)
                        {
                            ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeFadeOut));

                            __timer.IsRunning = false;
                        }
                        break;
                    }
                case FadeScreenMode.ModeComplete:
                    {
                        if(__CompleteModeSwicher)
                        {
                            __alpha = MAX(__alpha + __tpf);

                            if (__alpha == MAX_TRANSPARENCY)
                            {
                                ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeFadeIn));
                                __CompleteModeSwicher = false;
                            }
                        }
                        else
                        {
                            __alpha = MIN(__alpha - __tpf);

                            if (__alpha == 0)
                            {
                                ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeComplete));
                                __timer.IsRunning = false;
                            }
                        }
                        break;
                    }
            }

            Color color;
            color = __tdx.BoxColor;
            color.A = __alpha;
            __tdx.BoxColor = color;

            Console.WriteLine("__timer_Elapsed");
        }

        private static byte MAX(float v)
        {
            if (v > MAX_TRANSPARENCY)
                return MAX_TRANSPARENCY;
            else
                return Convert.ToByte(v);
        }

        private static byte MIN(float v)
        {
            if (v < 0)
                return 0;
            else
                return Convert.ToByte(v);
        }

        private void __FadeScreen(int ms, FadeScreenMode mode, Color tocolor)
        {
            if (__timer.IsRunning)
                __timer.IsRunning = false;

            if (ms < FRAMES)
                ms = FRAMES;

            __alpha = ((mode == FadeScreenMode.ModeFadeIn || mode == FadeScreenMode.ModeComplete) ? (byte)0 : MAX_TRANSPARENCY);
            __mode = mode;
            __color = tocolor;
            __CompleteModeSwicher = true;

            if (mode == FadeScreenMode.ModeComplete)
                ms /= 2;

            int frames = (ms / FRAMES);
            __tpf = MAX_TRANSPARENCY / frames;

            if (__tpf <= 0)
                __tpf = 10;

            tocolor.A = __alpha;
            __tdx.BoxColor = tocolor;

            __tdx.Show();
            __timer.IsRunning = true;
        }

        private void Player_Disconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            __timer.IsRunning = false;
            __timer.Dispose();
        }
    }
}
