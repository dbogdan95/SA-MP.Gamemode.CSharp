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
    public class MessageBox
    {
        private PlayerTextDraw __box;
        private Timer __timer = new Timer();

        public MessageBox(Player player)
        {
            __box = new PlayerTextDraw(player)
            {
                Position = new Vector2(25.500000, 170.312500),
                LetterSize = new Vector2(0.399500, 1.538750),
                Height = 0.000000f,
                Width = 181.000000f,
                Alignment = SampSharp.GameMode.Definitions.TextDrawAlignment.Left,
                ForeColor = -1,
                UseBox = true,
                Shadow = 0,
                Outline = 0,
                BackColor = 255,
                BoxColor = 150,
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

            if(__timer.Enabled)
                __timer.Stop();
            
            __timer.Interval = forms;
            __timer.Elapsed += (sender, e) =>
            {
                __box.Hide();
                __timer.Stop();
            };
            __timer.Start();
        }
    }
}
