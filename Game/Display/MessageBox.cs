using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using Game.World.Players;
using SampSharp.GameMode.SAMP;
using System;

namespace Game.Display
{
    public class MessageBox
    {
        private PlayerTextDraw __box;
        private Timer __timer = new Timer(100, true);

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

            if(__timer.IsRunning)
                __timer.IsRunning = false;
            
            __timer.Interval = TimeSpan.FromMilliseconds(forms);
            __timer.Tick += (sender, e) =>
            {
                __box.Hide();
                __timer.IsRunning = false;
            };
            __timer.IsRunning = true;
        }
    }
}
