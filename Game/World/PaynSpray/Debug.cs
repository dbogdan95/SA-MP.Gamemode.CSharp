using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.PaynSpray
{
    partial class PaynSpray
    {
        static List<DynamicTextLabel> dev_labels = new List<DynamicTextLabel>();
        static bool __devOn = false;

        public static void ToggleDeveloper()
        {
            __devOn = !__devOn;

            if (__devOn)
            {
                foreach (PaynSpray paynspray in GetAll<PaynSpray>())
                {
                    string label = null;
                    label += "[Pay'n'Spray]\r\n";
                    label += "ID: " + paynspray.Id;

                    Vector3 v = new Vector3((paynspray.AreaMins.X + paynspray.AreaMaxs.X) / 2,
                                            (paynspray.AreaMins.Y + paynspray.AreaMaxs.Y) / 2,
                                            (paynspray.AreaMins.Z + paynspray.AreaMaxs.Z) / 2);

                    dev_labels.Add(new DynamicTextLabel(label, Color.Green, v, 50.0f));
                }
            }
            else
            {
                foreach (DynamicTextLabel d in dev_labels)
                    d.Dispose();

                dev_labels.Clear();
            }
        }
    }
}
