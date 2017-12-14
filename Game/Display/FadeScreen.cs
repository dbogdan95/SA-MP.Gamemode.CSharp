using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Display
{
    class FadeScreen
    {
        static int FRAMES = 100;
        static byte MAX_TRANSPARENCY = 0xFF;

        Player __player;
        byte __alpha;
        Color __color;
        int __frames;
        int __tpf;
        FadeScreenMode __mode;
        PlayerTextDraw __tdx;
        System.Timers.Timer __timer;

        public FadeScreen(Player player, int ms)
        {
            if (ms < FRAMES)
                ms = FRAMES;

            __player = player;
            __alpha = 0;
            __mode = FadeScreenMode.ModeFadeIn;

            __color = Color.Black;
            __frames = (ms / FRAMES);
            __tpf = MAX_TRANSPARENCY / __frames;

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

            Color black = __color;
            black.A = 0;

            __tdx.BoxColor = black;

            player.Disconnected += Player_Disconnected;
        }

        public FadeScreen(Player player, int ms, FadeScreenMode mode)
        {
            if (ms < FRAMES)
                ms = FRAMES;

            __player = player;
            __alpha = ((mode == FadeScreenMode.ModeFadeIn || mode == FadeScreenMode.ModeComplete) ? (byte)0 : MAX_TRANSPARENCY);
            __mode = mode;

            __color = Color.Black;
            __frames = (ms / FRAMES);
            __tpf = MAX_TRANSPARENCY / __frames;

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

            Color black = __color;
            black.A = 0;

            __tdx.BoxColor = black;

            player.Disconnected += Player_Disconnected;
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
            switch(__mode)
            {
                case FadeScreenMode.ModeFadeIn:
                    {
                        __alpha = MAX(__alpha + __tpf);

                        if(__alpha == MAX_TRANSPARENCY)
                        {
                            __timer.Stop();
                            __timer.Dispose();
                        }
                        break;
                    }
                case FadeScreenMode.ModeFadeOut:
                    {
                        __alpha = MIN(__alpha - __tpf);

                        if (__alpha == 0)
                        {
                            __timer.Stop();
                            __timer.Dispose();
                        }
                        break;
                    }
                case FadeScreenMode.ModeComplete:
                    {
                        break;
                    }
            }

            Color color;
            color = __tdx.BoxColor;
            color.A = __alpha;
            __tdx.BoxColor = color;

            Console.WriteLine("__timer_Elapsed");
        }

        private void Player_Disconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            __timer.Stop();
            __timer.Dispose();
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
    }
}
