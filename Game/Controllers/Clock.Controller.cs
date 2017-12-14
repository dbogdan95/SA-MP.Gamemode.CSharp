using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World
{
    class ClockController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.Initialized += Clock_OnInit;
            gameMode.PlayerConnected += Clock_OnPlayerConnected;
        }

        private void Clock_OnPlayerConnected(object sender, EventArgs e)
        {
            Clock.ShowClock(sender as Player);
        }

        private void Clock_OnInit(object sender, EventArgs e)
        {
            Clock.Start();
        }
    }
}
