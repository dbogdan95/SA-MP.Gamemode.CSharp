using SampSharp.GameMode.Controllers;
using System;
using System.Xml;
using SampSharp.GameMode;
using Game.World;
using Game.Core;
using System.Data;
using Game.World.Property;
using MySql.Data.MySqlClient;

namespace Game.Controllers
{
    class PropertyController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.Initialized += Property_OnInitialized;
        }

        private void Property_OnInitialized(object sender, EventArgs e)
        {
            int props = 0;
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM houses", conn);
                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    House h = new House(Interior.FromIndex((int)data["interior"]), new Vector3((float)data["x"], (float)data["y"], (float)data["z"]), (float)data["a"], (int)data["id"]);
                    h.Locked = (bool)data["locked"];
                    h.Deposit = (int)data["deposit"]; // we are using the method from base because we don't want to update the databse
                    h.Rent = (int)data["rent"];
                    h.Level = (int)data["level"];
                    props++;
                }
                data.Close();

                cmd = new MySqlCommand("SELECT * FROM business", conn);
                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Business b = new Business(Interior.FromIndex((int)data["interior"]), new Vector3((float)data["x"], (float)data["y"], (float)data["z"]), (float)data["a"], (int)data["id"]);
                    b.Locked = (bool)data["locked"];
                    b.Deposit = (int)data["deposit"];  // we are using the method from base because we don't want to update the databse
                    props++;
                }
                data.Close();
            }
            Console.WriteLine("** Loaded {0} properties from database.", props);
        }
    }
}
