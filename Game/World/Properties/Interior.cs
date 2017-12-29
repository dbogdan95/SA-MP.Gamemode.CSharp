using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.Streamer.World;
using System;
using System.Xml;

namespace Game.World.Properties
{
    public class Interior : Pool<Interior>
    {
        private int             __gameInterior;
        private string          __name;
        private Vector3         __pos;
        private float           __a;
        private DynamicPickup   __pickup;
        private DynamicArea     __area;

        public Interior(int gameInterior, string name, Vector3 pos, float a)
        {
            __gameInterior = gameInterior;
            __name = name;
            __pos = pos;
            __a = a;
            __pickup = new DynamicPickup(19523, 23, pos);
            __area = DynamicArea.CreateSphere(pos, 2.0f);

            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }

        private void __area_Leave(object sender, PlayerEventArgs e)
        {
            LeaveDoor?.Invoke(this, e);
        }
        private void __area_Enter(object sender, PlayerEventArgs e)
        {
            TouchDoor?.Invoke(this, e);
        }

        // Summary:
        //     Called when a player leave the area around the interior's door.
        public event EventHandler<PlayerEventArgs> LeaveDoor;
        // Summary:
        //     Called when a player enter the area around the interior's door.
        public event EventHandler<PlayerEventArgs> TouchDoor;
        // Summary:
        //     Gets the position of door.
        public Vector3 Position => __pos;
        // Summary:
        //     Gets the rotation.
        public float Angle => __a;
        // Summary:
        //     Gets the game interior.
        public int GameInterior => __gameInterior;

        public string Name { get => __name; set => __name = value; }

        // Summary:
        //     Gets the name of interior.
        public override string ToString()
        {
            return "Interior(Id:" + GetAll<Interior>().IndexOf(this) + " Name: " + __name + ")";
        }
        // Summary:
        //     Check if given id is valid for the interiors pool.
        public static bool ValidPoolID(int id)
        {
            return (id < GetAll<Interior>().Count && id > -1);
        }
        public static int Index(Interior interior)
        {
            if (interior == null)
                return -1;

            return GetAll<Interior>().IndexOf(interior);
        }
        public static Interior FromIndex(int idx)
        {
            if (!ValidPoolID(idx))
                return null;

            return GetAll<Interior>()[idx];
        }

        public static void Load(string xmlfile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlfile);

            int c = 0;
            foreach (XmlNode node in doc.DocumentElement)
            {
                new Interior(Convert.ToInt32(node.Attributes["id"].InnerText), node.InnerText, new Vector3(
                    Convert.ToSingle(node.Attributes["x"].InnerText),
                    Convert.ToSingle(node.Attributes["y"].InnerText),
                    Convert.ToSingle(node.Attributes["z"].InnerText)),
                    Convert.ToSingle(node.Attributes["a"].InnerText));

                c++;
            }
            Console.WriteLine("** Loaded {0} interiors from {1}.", c, xmlfile);
        }
    }
}
