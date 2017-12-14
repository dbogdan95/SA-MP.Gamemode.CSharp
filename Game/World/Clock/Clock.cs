using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Definitions;

namespace Game.World
{
    class Clock
    {
        private static System.Timers.Timer clockTimer;
        private static DateTime time;
        private static TextDraw txdClock = new TextDraw(new Vector2(627.500000, 406.799987), "14:46~n~30/11/2017 - JOI");

        public static void Start()
        {
            txdClock.LetterSize = new Vector2(0.385500, 1.573750);
            txdClock.Alignment = TextDrawAlignment.Right;
            txdClock.Shadow = 0;
            txdClock.Outline = 1;
            txdClock.ForeColor = Color.Orange;
            txdClock.BackColor = 51;
            txdClock.Font = TextDrawFont.Pricedown;
            txdClock.Proportional = true;

            clockTimer = new System.Timers.Timer(1000);
            clockTimer.Elapsed += new ElapsedEventHandler(Clock_OnTick);
            clockTimer.Start();
        }

        private static void Clock_OnTick(object sender, EventArgs e)
        {
            // TODO: actualizeaza doar daca minutele sunt diferite
            time = DateTime.Now;
            txdClock.Text = time.ToString("dd/MM/yyyy ~n~HH:mm");

            foreach (Player next in Player.GetAll<Player>())
            {
                next.SetTime(time.Hour, time.Minute);
            }
        }

        public static void ShowClock(Player player)
        {
            txdClock.Show(player);
        }
    }
}
