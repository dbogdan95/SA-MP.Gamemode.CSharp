using Game.World;
using SampSharp.GameMode;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class Util
    {
        public static double Absoluteangle(double angle)
        {
            while (angle < 0.0) angle += 360.0f;
            while (angle > 360.0) angle -= 360.0f;
            return angle;
        }

        public static double GetAngleToPoint(Vector2 v1, Vector2 v2)
        {
            return Absoluteangle(-(90 - (Math.Atan2((v2.Y - v1.Y), (v2.X - v1.X)))));
        }

        public static double GetAngleToPoint(Vector3 v1, Vector3 v2)
        {
            return Absoluteangle(-(90 - (Math.Atan2((v2.Y - v1.Y), (v2.X - v1.X)))));
        }

        public static void DecodePanels(int panels, out int front_left_panel, out int front_right_panel, out int rear_left_panel, out int rear_right_panel, out int windshield, out int front_bumper, out int rear_bumper)
        {
            front_left_panel = panels & 15;
            front_right_panel = panels >> 4 & 15;
            rear_left_panel = panels >> 8 & 15;
            rear_right_panel = panels >> 12 & 15;
            windshield = panels >> 16 & 15;
            front_bumper = panels >> 20 & 15;
            rear_bumper = panels >> 24 & 15;
        }

        public static void DecodeDoors(int doors, out int bonnet, out int boot, out int driver_door, out int passenger_door)
        {
            bonnet = doors & 7;
            boot = doors >> 8 & 7;
            driver_door = doors >> 16 & 7;
            passenger_door = doors >> 24 & 7;
        }

        public static void DecodeLights(int lights, out int front_left_light, out int front_right_light, out int back_lights)
        {
            front_left_light = lights & 1;
            front_right_light = lights >> 2 & 1;
            back_lights = lights >> 6 & 1;
        }

        public static void DecodeTires(int tires, out int rear_right_tire, out int front_right_tire, out int rear_left_tire, out int front_left_tire)
        {
            rear_right_tire = tires & 1;
            front_right_tire = tires >> 1 & 1;
            rear_left_tire = tires >> 2 & 1;
            front_left_tire = tires >> 3 & 1;
        }

        public static string FormatNumber(int number)
        {
            return "$"+String.Format("{0:n}", number);
        }

        public static void PlaySoundInRangeOfPoint(Sounds snd, Vector3 src, float range)
        {
            foreach (Player next in BasePlayer.GetAll<Player>())
            {
                if(src.DistanceTo(next.Position) < range)
                {
                    next.PlaySound((int)snd, src);
                }
            }
        }
    }
}
