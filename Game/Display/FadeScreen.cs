using Game.World.Players;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;

namespace Game.Display
{
    class FadeScreen
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
        System.Timers.Timer __timer;

        bool __CompleteModeSwicher;

        public FadeScreen(Player player, int ms)
        {
            __Fade(player, ms, FadeScreenMode.ModeFadeIn, Color.Black);
        }

        public FadeScreen(Player player, int ms, FadeScreenMode mode)
        {
            __Fade(player, ms, mode, Color.Black);
        }

        public FadeScreen(Player player, int ms, FadeScreenMode mode, Color tocolor)
        {
            __Fade(player, ms, mode, tocolor);
        }

        ~FadeScreen()
        {
            Console.WriteLine("Kill FadeScreen");
        }

        public void Start()
        {
            __timer = new System.Timers.Timer(FRAMES);
            __timer.Elapsed += __timer_Elapsed;

            __tdx.Show();
            __timer.Start();
        }

        private void __timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool end = false;
            switch(__mode)
            {
                case FadeScreenMode.ModeFadeIn:
                    {
                        __alpha = MAX(__alpha + __tpf);

                        if(__alpha == MAX_TRANSPARENCY)
                        {
                            ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeFadeIn));

                            __timer.Stop();
                            __timer.Dispose();
                            end = true;
                        }
                        break;
                    }
                case FadeScreenMode.ModeFadeOut:
                    {
                        __alpha = MIN(__alpha - __tpf);

                        if (__alpha == 0)
                        {
                            ScreenFadeEnd?.Invoke(__player, new FadeScreenEventArgs(__player, FadeScreenMode.ModeFadeOut));

                            __timer.Stop();
                            __timer.Dispose();
                            end = true;
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
                                __timer.Stop();
                                __timer.Dispose();
                                end = true;
                            }
                        }
                        break;
                    }
            }

            Color color;
            color = __tdx.BoxColor;
            color.A = __alpha;
            __tdx.BoxColor = color;

            if (end)
                __tdx.Dispose();

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

        private void __Fade(Player player, int ms, FadeScreenMode mode, Color tocolor)
        {
            if (ms < FRAMES)
                ms = FRAMES;

            __player = player;
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

            __tdx = new PlayerTextDraw(player)
            {
                Position = new Vector2(-20.0, -20.0),
                Text = "_",
                Font = SampSharp.GameMode.Definitions.TextDrawFont.Pricedown,
                LetterSize = new Vector2(680.0, 500.0),
                UseBox = true
            };

            tocolor.A = __alpha;
            __tdx.BoxColor = tocolor;

            player.Disconnected += Player_Disconnected;
        }

        private void Player_Disconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            __timer.Stop();
            __timer.Dispose();
        }
    }
}
