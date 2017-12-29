using SampSharp.GameMode;
using SampSharp.Streamer.World;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Game.World
{
    class Mapping
    {
        public static void Load(string xmlfile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);

            int c = 0;
            foreach (XmlNode node in doc.GetElementsByTagName("dynamicObjects"))
            {
                foreach (XmlNode obj in node.ChildNodes)
                {
                    if (obj.NodeType == XmlNodeType.Comment)
                        continue;

                    new DynamicObject(Convert.ToInt32(obj.Attributes["id"].InnerText),

                    new Vector3(
                    Convert.ToSingle(obj.Attributes["x"].InnerText),
                    Convert.ToSingle(obj.Attributes["y"].InnerText),
                    Convert.ToSingle(obj.Attributes["z"].InnerText)),

                    new Vector3(
                    Convert.ToSingle(obj.Attributes["rx"].InnerText),
                    Convert.ToSingle(obj.Attributes["ry"].InnerText),
                    Convert.ToSingle(obj.Attributes["rz"].InnerText)));

                    c++;
                }
            }

            Console.WriteLine("** Loaded {0} dynamic objects from {1}.", c, xmlfile);
        }
    }
}
