using Game.World.Players;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Helpers;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Tools;
using System;

namespace Game.World.Vehicles
{
    public class Hud : Disposable
    {
        static TextDraw lockTextdraw;
        static TextDraw lightsTexdraw;

        static Hud()
        {
            lockTextdraw = new TextDraw
            {
                Text = "mdl-10000:car_locked",
                Position = new Vector2(605.000000, 327.000000),
                Font = TextDrawFont.DrawSprite,
                Height = 25.000000f,
                Width = 18.000000f
            };

            lightsTexdraw = new TextDraw
            {
                Text = "mdl-10000:headlights",
                Position = new Vector2(582.000000, 323.000000),
                Font = TextDrawFont.DrawSprite,
                Height = 28.000000f,
                Width = 23.500000f
            };
        }

        private Player __player;
        private PlayerTextDraw __fuelGrid;
        private PlayerTextDraw __speedo;
        private PlayerTextDraw __fuelBar;
        private PlayerTextDraw __cursor;

        private bool __lightsOn;
        private bool __lockOn;

        public Hud(Player player)
        {
            __player = player;
            __fuelGrid = new PlayerTextDraw(__player)
            {
                Text = "mdl-10000:fuel_meter",
                Position = new Vector2(521.000000, 344.000000),
                Font = TextDrawFont.DrawSprite,
                LetterSize = new Vector2(0.600000, 2.000000),
                Height = 29.500000f,
                Width = 112.500000f,
                Outline = 0,
                Shadow = 0,
                Alignment = TextDrawAlignment.Left,
                ForeColor = -1,
                BackColor = 255,
                UseBox = false,
                Proportional = true,
                AutoDestroy = true
            };

            __speedo = new PlayerTextDraw(__player)
            {
                Text = "0 km/h",
                Position = new Vector2(627.000000, 366.000000),
                Font = TextDrawFont.Slim,
                LetterSize = new Vector2(0.233330, 1.199996),
                Outline = 0,
                Shadow = 1,
                Alignment = TextDrawAlignment.Right,
                ForeColor = -1,
                BackColor = 255,
                UseBox = false,
                Proportional = true,
                AutoDestroy = true
            };

            Color gray = Color.Gray;
            gray.A = 80;

            __fuelBar = new PlayerTextDraw(__player)
            {
                Position = new Vector2(530.000000, 360.000000),
                LetterSize = new Vector2(0.600000, 0.400000),
                Width = 526.000000f,
                Height = 15.500000f,
                Shadow = 0,
                BoxColor = gray,
                UseBox = true
            };

            __cursor = new PlayerTextDraw(__player)
            {
                Text = "I",
                Position = new Vector2(527.000000, 349.000000),
                Font = TextDrawFont.Normal,
                LetterSize = new Vector2(0.258333, 1.300000),
                ForeColor = -8388353,
                Shadow = 0
            };
        }

        public void Show()
        {
            AssertNotDisposed();

            if (__player?.Vehicle == null)
                throw new Exception("Player must be in a vehicle");

            __fuelGrid.Show();
            __fuelBar.Show();
            __speedo.Show();
            __cursor.Show();

            __lightsOn = false;
            __lockOn = false;
        }

        public void Hide()
        {
            AssertNotDisposed();

            __fuelGrid.Hide();
            __fuelBar.Hide();
            __speedo.Hide();
            __cursor.Hide();
        }

        public void LightHud(bool b)
        {
            AssertNotDisposed();

            if (b)
            {
                if(__lockOn)
                    lightsTexdraw.Position = new Vector2(582.000000, 323.000000); // initial position
                else
                    lightsTexdraw.Position = new Vector2(605.000000, 327.000000); // change to lock's position

                lightsTexdraw.Show(__player);
            }
            else
                lightsTexdraw.Hide(__player);

            __lightsOn = b;
        }

        public void LockHud(bool b)
        {
            AssertNotDisposed();

            if (b)
            {
                if (__lightsOn)
                    lightsTexdraw.Position = new Vector2(582.000000, 323.000000); // initial position

                lockTextdraw.Show(__player);
            }
            else
            {
                if(__lightsOn)
                    lightsTexdraw.Position = new Vector2(605.000000, 327.000000); // change to lock's position

                lockTextdraw.Hide(__player);
            }
            __lockOn = b;
        }

        public void SpeedoValue(double speed)
        {
            AssertNotDisposed();
            __speedo.Text = speed.ToString("F") + " km/h";
        }

        public void FuelValue(float fuel, float max)
        {
            AssertNotDisposed();
            
            float lerp = MathHelper.Lerp(526.0f, 624.0f, fuel / max);

            __fuelBar.Width = lerp;
            __cursor.Position = new Vector2(lerp, 349.000000);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                __speedo.Hide();
                __fuelBar.Hide();
                lockTextdraw.Hide(__player);
                lightsTexdraw.Hide(__player);
            }
        }
    }
}
