using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Game.World.Zones
{
    class Zone
    {
        private string __name;
        //private Vector3 __mins;
        //private Vector3 __maxs;
        private DynamicArea __area;

        public Zone(string name, Vector3 min, Vector3 max)
        {
            __name = name;
            //__mins = min;
            //__maxs = max;

            __area = DynamicArea.CreateCube(min, max, interiorid: 0);
            __area.Enter += __area_Enter;
        }

        private void __area_Enter(object sender, SampSharp.GameMode.Events.PlayerEventArgs e)
        {
            e.Player.SendClientMessage(__name);
        }

        public static void Load(string xmlfile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);

            int c = 0;
            foreach (XmlNode node in doc.DocumentElement)
            {
                new Zone(node.Attributes["name"].InnerText,
                    new Vector3(
                        Convert.ToSingle(node.Attributes["minx"].InnerText),
                        Convert.ToSingle(node.Attributes["miny"].InnerText),
                        Convert.ToSingle(node.Attributes["minz"].InnerText)),

                    new Vector3(
                        Convert.ToSingle(node.Attributes["maxx"].InnerText),
                        Convert.ToSingle(node.Attributes["maxy"].InnerText),
                        Convert.ToSingle(node.Attributes["maxz"].InnerText)));

                c++;
            }
            Console.WriteLine("** Loaded {0} zones from {1}.", c, xmlfile);
        }
    }
}
