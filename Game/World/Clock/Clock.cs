using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using System;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Definitions;
using Game.World.Players;
using System.Linq;

namespace Game.World
{
    class Clock
    {
        private static Timer clockTimer;
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

            clockTimer = new Timer(1000, true);
            clockTimer.Tick += ClockTimer_Tick;
        }

        private static void ClockTimer_Tick(object sender, EventArgs e)
        {
            // TODO: actualizeaza doar daca minutele sunt diferite
            time = DateTime.Now;
            txdClock.Text = time.ToString("dd/MM/yyyy ~n~HH:mm");

            foreach (Player next in Player.GetAll<Player>().ToArray())
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
