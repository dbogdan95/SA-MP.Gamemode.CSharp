using SampSharp.GameMode.Controllers;
using System;
using SampSharp.GameMode;
using Game.Core;
using MySql.Data.MySqlClient;
using Game.World.Properties;

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
            //new BusinessType(1, 55, "DealerShip", 100000);
            //new BusinessType(2, 6, "Ammunition", 50000);
            //new BusinessType(3, 63, "Pay'n'Spray", 40000);
            //new BusinessType(4, 27, "ModShop", 30000);
            //new BusinessType(5, 10, "Burget Shot", 60000);
            //new BusinessType(6, 14, "Cluckin Bell", 60000);
            //new BusinessType(7, 29, "Pizza Stack", 60000);
            //new BusinessType(8, 22, "Hospital", 500000);
            //new BusinessType(9, 48, "Night Club", 50000);
            //new BusinessType(10, 21, "Sex Shot", 30000);
            //new BusinessType(11, 7, "Barber", 20000);
            //new BusinessType(12, 52, "Bank", 40000);

            int props = 0;
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT A.*, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior, C.price, B.id AS owner " +
                    "FROM houses as A " +
                    "LEFT JOIN players as B ON A.id = B.house " +
                    "INNER JOIN properties AS C ON c.id = A.baseProperty", conn);

                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    House h = new House(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"))
                    {
                        Locked = data.GetBoolean("locked"),
                        Deposit = data.GetInt32("deposit"),
                        Rent = data.GetInt32("rent"),
                        Level = data.GetInt32("level"),
                        Owner = data["owner"] is DBNull ? 0 : data.GetInt32("owner"),
                        Price = data.GetInt32("price")
                    };
                    h.UpdateLabel();
                    props++;
                }
                data.Close();

                cmd = new MySqlCommand("SELECT A.*, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior, C.price, B.id AS owner " +
                    "FROM business as A " +
                    "LEFT JOIN players as B ON A.id = B.business " +
                    "INNER JOIN properties AS C ON c.id = A.baseProperty", conn);

                data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Business b = new Business(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"))
                    {
                        Locked = data.GetBoolean("locked"),
                        Deposit = data.GetInt32("deposit"),
                        BizzType = BusinessType.Find(data.GetInt32("type")),
                        Domainid = data.GetInt32("domainid"),
                        Owner = data["owner"] is DBNull ? 0 : data.GetInt32("owner"),
                        Price = data.GetInt32("price")
                    };
                    b.UpdateLabel();
                    props++;
                }
                data.Close();
            }
            Console.WriteLine("** Loaded {0} properties from database.", props);
        }
    }
}
