using SampSharp.GameMode.Controllers;
using System;
using System.Xml;
using SampSharp.GameMode;
using Game.World;
using Game.Core;
using System.Data;
using Game.World.Property;
using Game.World.Property.Business;
using Game.World.Property.House;
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
            new BusinessType(1, 55, "DealerShip");
            new BusinessType(2, 6, "Ammunition");
            new BusinessType(3, 63, "Pay'n'Spray");
            new BusinessType(4, 27, "ModShop");
            new BusinessType(5, 10, "Burget Shot");
            new BusinessType(6, 14, "Cluckin Bell");
            new BusinessType(7, 29, "Pizza Stack");
            new BusinessType(8, 22, "Hospital");
            new BusinessType(9, 48, "Night Club");
            new BusinessType(10, 21, "Sex Shot");
            new BusinessType(11, 7, "Barber");
            new BusinessType(12, 52, "Bank");

            int props = 0;
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM houses", conn);
                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    House h = new House(Interior.FromIndex((int)data["interior"]), new Vector3((float)data["x"], (float)data["y"], (float)data["z"]), (float)data["a"], (int)data["id"]);
                    h.Locked = (bool)data["locked"];
                    h.Deposit = (int)data["deposit"];
                    h.Rent = (int)data["rent"];
                    h.Level = (int)data["level"];
                    h.Owner = (int)data["owner"];
                    h.UpdateLabel();
                    props++;
                }
                data.Close();

                cmd = new MySqlCommand("SELECT * FROM business", conn);
                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Business b = new Business(Interior.FromIndex((int)data["interior"]), new Vector3((float)data["x"], (float)data["y"], (float)data["z"]), (float)data["a"], (int)data["id"]);
                    b.Locked = (bool)data["locked"];
                    b.Deposit = (int)data["deposit"]; 
                    b.BizzType = BusinessType.Find((int)data["type"]);
                    b.Domainid = (int)data["domainid"];
                    props++;
                }
                data.Close();
            }
            Console.WriteLine("** Loaded {0} properties from database.", props);
        }
    }
}
