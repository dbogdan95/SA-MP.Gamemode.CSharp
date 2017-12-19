using SampSharp.GameMode.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using System.Timers;
using Game.World.Players;

namespace Game.Display
{
    class MessageBox : IController, IEventListener
    {
        private PlayerTextDraw __box;

        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.PlayerConnected += GameMode_PlayerConnected;
        }

        private void GameMode_PlayerConnected(object sender, EventArgs e)
        {
            __box = new PlayerTextDraw(sender as Player)
            {
                Position = new Vector2(25.500000, 170.312500),
                LetterSize = new Vector2(0.399500, 1.538750),
                Alignment = SampSharp.GameMode.Definitions.TextDrawAlignment.Left,
                ForeColor = -1,
                UseBox = true,
                Shadow = 0,
                Outline = 0,
                BackColor = 255,
                Font = SampSharp.GameMode.Definitions.TextDrawFont.Normal,
                Proportional = true
            };
        }

        public void Show(string txt)
        {
            __Box(txt, 5000);
        }

        public void Show(string txt, int forms)
        {
            __Box(txt, forms);
        }

        private void __Box(string txt, int forms)
        {
            __box.Text = txt;
            __box.Show();

            Timer t = new Timer(forms);
            t.Elapsed += (sender, e) =>
            {
                __box.Hide();
                t.Stop();
                t.Dispose();
            };
            t.Start();
        }
    }
}
