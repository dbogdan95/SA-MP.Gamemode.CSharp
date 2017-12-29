using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.Streamer.Definitions;

using Game.World;
using Game.Controllers;
using Game.World.Items;
using Game.World.Properties;
using Game.World.GasStations;
using Game.World.Players;
using Game.World.PaynSprays;
using Game.World.Vehicles;
using Game.World.Zones;
using SampSharp.Streamer.World;
using SampSharp.GameMode.SAMP;
using Game.Factions;
using Game.World.FastFoods;

namespace Game
{
    class Game : BaseMode
    {
        TextDraw serverName = new TextDraw
        {
            Text = "RPG.FMM.PLM",
            Position = new Vector2(628.000000, 433.562500),
            LetterSize = new Vector2(0.385500, 1.5),
            Alignment = TextDrawAlignment.Right,
            ForeColor = Color.Silver,
            Shadow = 0,
            Outline = 1,
            BackColor = 51,
            Font = TextDrawFont.Pricedown,
            Proportional = true
        };

        static ItemType cornItem = new ItemType("Porumb", Common.CORN_MODELID);

        protected override void OnInitialized(EventArgs e)
        {
            SampSharp.Streamer.Streamer.DestroyAllItems(StreamType.All);

            SetGameModeText("cioak");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            ManualVehicleEngineAndLights();

            Item item = new Item(cornItem, Vector3.Zero);
    
            new PaynSpray(1, -1002, new Vector3(1024.9884, -1029.3324, 35.1272), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(1024.9884, -1029.3324, 33.1772), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(1021.8430, -1018.5289, 31.1826), new Vector3(1027.9781, -1029.1014, 36.8535), new Vector3(1024.6614, -1035.0811, 34.7631)).Open();
            new PaynSpray(2, -1003, new Vector3(488.2576, -1734.8486, 14.6693), new Vector3(0.0000, 90.0000, -97.9200), new Vector3(488.2576, -1734.8486, 12.3906), new Vector3(0.0000, 0.0000, -97.9200), new Vector3(489.7542, -1747.4993, 10.2083), new Vector3(484.9799, -1734.7427, 16.7146), new Vector3(489.6070, -1728.6317, 14.9308)).Open();
            new PaynSpray(3, -1004, new Vector3(-1420.5447, 2591.2354, 60.5496), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-1420.5447, 2591.2354, 57.7422), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-1416.1711, 2590.9380, 54.8367), new Vector3(-1424.9185, 2576.6777, 60.2768), new Vector3(-1419.8292, 2598.4800, 60.3985));
            new PaynSpray(4, -1005, new Vector3(-1904.5099, 277.6387, 45.7687), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-1904.5099, 277.6387, 42.9531), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-1900.1495, 292.2030, 40.0500), new Vector3(-1908.8933, 278.7156, 45.5111), new Vector3(-1905.0630, 270.2467, 46.0506));
            new PaynSpray(5, -1006, new Vector3(-2425.6450, 1028.2318, 55.0539), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-2425.6450, 1028.2318, 52.2813), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-2430.0173, 1013.7064, 49.3985), new Vector3(-2421.3354, 1027.4117, 54.8807), new Vector3(-2425.1062, 1035.7886, 55.6979));
            new PaynSpray(6, -1007, new Vector3(1968.4006, 2162.5015, 14.0288), new Vector3(0.0000, 90.0000, 0.0000), new Vector3(1968.4006, 2162.5015, 12.0938), Vector3.Zero, new Vector3(1983.1675, 2166.8013, 9.9969), new Vector3(1968.7124, 2158.6880, 15.6400), new Vector3(1962.0724, 2162.7275, 14.1024));
            new PaynSpray(7, -1002, new Vector3(2071.5762, -1831.4224, 16.4406), new Vector3(0.0000, 90.0000, 0.0000), new Vector3(2071.5564, -1831.4203, 14.5625), Vector3.Zero, new Vector3(2056.6753, -1835.9222, 12.4891), new Vector3(2070.4165, -1827.1287, 18.0190), new Vector3(2076.6843, -1831.5831, 16.5109)).Open();
            new PaynSpray(8, -1004, new Vector3(-100.0399, 1111.4144, 24.4802), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-100.0399, 1111.4144, 21.6406), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-104.4104, 1125.9390, 18.6544), new Vector3(-95.6366, 1112.2601, 24.2028), new Vector3(-100.8332, 1103.6481, 23.6690));
            new PaynSpray(9, -1008, new Vector3(720.0145, -462.4834, 18.6588), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(720.0145, -462.4834, 16.8594), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(715.7748, -447.3318, 14.9643), new Vector3(724.2939, -461.0360, 20.4023), new Vector3(719.7719, -467.8614, 18.5736)).Open();
            
            new GasStation(1, new Vector3(995.5288, -946.8690, 41.0169), new Vector3(1011.9250, -928.9503, 46.5377));
            new GasStation(2, new Vector3(1950.2308, -1782.4164, 17.8519), new Vector3(1932.8807, -1763.3589, 12.0884));
            new GasStation(3, new Vector3(-98.8744, -1173.8379, 5.3181), new Vector3(-82.8693, -1164.7268, 1.1242));
            new GasStation(4, new Vector3(-1608.8652, -2723.4731, 47.4580), new Vector3(-1603.7380, -2704.1912, 52.7388));
            new GasStation(5, new Vector3(-2405.5562, 958.0877, 44.2969), new Vector3(-2416.3118, 992.0084, 50.2754)).Off();
            new GasStation(6, new Vector3(-2251.0879, -2562.7524, 30.9237), new Vector3(-2237.7690, -2559.9014, 36.2992));
            new GasStation(7, new Vector3(-1659.4701, 417.4124, 6.1868), new Vector3(-1695.6837, 412.9560, 10.5386)).Off();
            new GasStation(8, new Vector3(2191.8389, 2467.4988, 9.8208), new Vector3(2211.9209, 2482.1394, 14.6517)).Off();
            new GasStation(9, new Vector3(629.7487, 1678.8574, 5.9884), new Vector3(598.1221, 1705.9727, 10.9382)).Off();
            new GasStation(10, new Vector3(2122.1111, 910.3763, 9.7593), new Vector3(2107.5552, 929.7073, 14.6152)).Off();
            new GasStation(11, new Vector3(2647.6121, 1117.0409, 9.8205), new Vector3(2632.7041, 1096.7050, 14.6148)).Off();
            new GasStation(12, new Vector3(2155.4351, 2737.9014, 9.8211), new Vector3(2140.4011, 2757.5012, 14.6144)).Off();
            new GasStation(13, new Vector3(1606.2572, 2188.6067, 9.8207), new Vector3(1586.8804, 2208.7385, 14.6514)).Off();
            new GasStation(14, new Vector3(-1463.0481, 1858.7996, 31.6321), new Vector3(-1479.0486, 1869.0215, 36.8829)).Off();
            new GasStation(15, new Vector3(650.8358, -576.4760, 15.3088), new Vector3(660.3215, -553.9163, 20.5575));
            new GasStation(16, new Vector3(1386.9375, 454.7686, 18.9276), new Vector3(1378.8147, 467.7030, 22.1437));

            FastFoodFactory.Create(FastFoodTypes.TypePizzaStack, 1, new Vector3(2111.9417, -1807.8501, 12.5536));

            new GlobalObject(-1010, new Vector3(1571.6016, -1675.7500, 35.6677), Vector3.Zero);
            new GlobalObject(-1011, new Vector3(1571.3461, -1670.9417, 17.6223), Vector3.Zero);

            new DynamicObject(-1012, new Vector3(1555.8922, -1674.0765, 15.1955), new Vector3(0.0000, 0.0000, -90.0000));
            new DynamicObject(-1012, new Vector3(1555.9153, -1677.1036, 15.1955), new Vector3(0.0000, 0.0000, 90.0000));

            new GlobalObject(-1013, new Vector3(1391.1254, -1318.1013, 24.6661), Vector3.Zero);
            new GlobalObject(-1014, new Vector3(1385.6791, -1298.5975, 14.7725), new Vector3(0.0000, 0.0000, -90.0000));

            // GasStation - Vinewood 24/7
            new GlobalObject(-1015, new Vector3(1018.1906, -908.9792, 43.6484), new Vector3(0.0049, 0.0000, 7.6800));
            new GlobalObject(-1016, new Vector3(1006.2781, -919.0173, 43.3264), new Vector3(0.0000, 0.0000, 8.5200));

            // Canal
            new GlobalObject(-1017, new Vector3(2576.87, -1506.83, 15.6), Vector3.Zero);
            new GlobalObject(-1018, new Vector3(2476.77, -1490.42, 15.6), Vector3.Zero);
            new GlobalObject(-1019, new Vector3(2374.25, -1491.9, 15.6), Vector3.Zero);
            new GlobalObject(-1020, new Vector3(2310.4, -1470.11, 15.6), Vector3.Zero);
            new GlobalObject(-1021, new Vector3(2201.3, -1476.94, 15.6), Vector3.Zero);
            new GlobalObject(-1022, new Vector3(1987.8, -1360.6, -30), Vector3.Zero);
            new GlobalObject(-1022, new Vector3(1950.85, -1360.6, -30.002), Vector3.Zero);
            new GlobalObject(-1023, new Vector3(2087.28, -1491.94, -10.05), Vector3.Zero);
            new GlobalObject(-1024, new Vector3(2063.55, -1413.1, 15.6), Vector3.Zero);
            new GlobalObject(-1025, new Vector3(2177.51, -1468.21, -10.2), Vector3.Zero);
            new GlobalObject(-1026, new Vector3(2374.25, -1510.62, 4.7), Vector3.Zero);
            new GlobalObject(-1027, new Vector3(2385.01, -1479.6, -5), Vector3.Zero);
            new GlobalObject(-1028, new Vector3(2457.25, -1472.6, 13.91), Vector3.Zero);
            new GlobalObject(-1029, new Vector3(1956.73, -1335.37, 7.5), Vector3.Zero);
            new GlobalObject(-1030, new Vector3(1897.26, -1257.06, -18.88), Vector3.Zero);
            new GlobalObject(-1031, new Vector3(1931.25, -1168.85, -19.1), Vector3.Zero);

            //new GlobalObject(-1034, new Vector3(1875.56, -1323.5, -18.999), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(1890.79, -1270.33, -27.44), new Vector3(0, 0, 89.99989));
            new GlobalObject(-1034, new Vector3(1975.54, -1280.44, -27.44), new Vector3(0, 0, 269.9995));
            new GlobalObject(-1034, new Vector3(1990.77, -1226.73, -18.95), new Vector3(0, 0, 179.9998));
            //new GlobalObject(-1034, new Vector3(1967.97, -1206.4, -18.95), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(1983.2, -1152.9, -27.2), new Vector3(0, 0, 89.99989));
           // new GlobalObject(-1034, new Vector3(2067.95, -1163.01, -27.2), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(2088.33, -1140.11, -27.2), new Vector3(0, 0, 89.99989));
          //  new GlobalObject(-1034, new Vector3(2173.06, -1150.22, -27.2), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(2193.43, -1127.33, -27.2), new Vector3(0, 0, 89.99989));
           // new GlobalObject(-1034, new Vector3(2278.16, -1137.43, -27.2), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(2293.39, -1083.7, -18.7), new Vector3(0, 0, -180.0000));
            //new GlobalObject(-1034, new Vector3(2270.5, -1063.2, -18.7), Vector3.Zero);
            new GlobalObject(-1034, new Vector3(2280.61, -978.523, -18.7), new Vector3(0, 0, -180.0000));

            new GlobalObject(-1035, new Vector3(1819.97, -1362.5, -37.91), Vector3.Zero);
            new GlobalObject(-1037, new Vector3(2286.1719, -974.8047, 30.5000), Vector3.Zero);
            new GlobalObject(-1038, new Vector3(2587.0781, -1589.4453, 15.2734), Vector3.Zero);

            // Pizza stack
            new GlobalObject(-1040, new Vector3(2112.9375, -1797.0938, 19.3359), Vector3.Zero);
            new GlobalObject(-1039, new Vector3(2113.2525, -1802.9898, 14.6708), Vector3.Zero);

            Zone.Load(@"zones.xml");
            Interior.Load(@"interiors.xml");
            House.Load();
            Business.Load();
            Faction.Load();
            Generic.Load();
            Mapping.Load("mapping.xml");

            base.OnInitialized(e);
        }

        protected override void LoadControllers(ControllerCollection controllers)
        {
            controllers.Add(new CornPlantController());
            controllers.Add(new ClockController());
            controllers.Add(new ItemController());
            controllers.Add(new PropertyController());
            controllers.Add(new VehicleController());

            base.LoadControllers(controllers);
        }

        protected override void OnPlayerConnected(BasePlayer player, EventArgs e)
        {
            base.OnPlayerConnected(player, e);
            player.ToggleClock(false);
            serverName.Show(player);

            GlobalObject.Remove(player, 5422, new Vector3(2071.4766, -1831.4219, 14.5625), 0.25f);
            GlobalObject.Remove(player, 5856, new Vector3(1024.9844, -1029.3516, 33.1953), 0.25f);
            GlobalObject.Remove(player, 6400, new Vector3(488.2813, -1734.6953, 12.3906), 0.25f);
            GlobalObject.Remove(player, 11319, new Vector3(-1904.5313, 277.8984, 42.9531), 0.25f);
            GlobalObject.Remove(player, 9625, new Vector3(-2425.7266, 1027.9922, 52.2813), 0.25f);
            GlobalObject.Remove(player, 7891, new Vector3(1968.7422, 2162.4922, 12.0938), 0.25f);
            GlobalObject.Remove(player, 3294, new Vector3(-1420.5469, 2591.1563, 57.7422), 0.25f);
            GlobalObject.Remove(player, 3294, new Vector3(-100.0000, 1111.4141, 21.6406), 0.25f);
            GlobalObject.Remove(player, 13028, new Vector3(720.0156, -462.5234, 16.8594), 0.25f);

            GlobalObject.Remove(player, 4064, new Vector3(1571.6016, -1675.7500, 35.6797), 0.25f);
            GlobalObject.Remove(player, 1536, new Vector3(1555.9297, -1677.1250, 15.1797), 5.0f);
            GlobalObject.Remove(player, 3976, new Vector3(1571.6016, -1675.7500, 35.6797), 0.25f);

            GlobalObject.Remove(player, 4632, new Vector3(1391.1250, -1318.0938, 24.6641), 0.25f);
            GlobalObject.Remove(player, 4552, new Vector3(1391.1250, -1318.0938, 24.6641), 0.25f);
            GlobalObject.Remove(player, 1537, new Vector3(1369.3984, -1281.2969, 12.5391), 0.25f);
            GlobalObject.Remove(player, 1533, new Vector3(1369.3984, -1278.2813, 12.5391), 0.25f);

            GlobalObject.Remove(player, 5898, new Vector3(1018.1641, -908.9766, 43.6484), 0.25f);
            GlobalObject.Remove(player, 5853, new Vector3(1018.1641, -908.9766, 43.6484), 0.25f);

            GlobalObject.Remove(player, 1522, new Vector3(1000.8750, -919.1094, 41.2891), 0.25f);

            GlobalObject.Remove(player, 5598, new Vector3(1914.2109, -1300.0547, 30.7266), 0.25f);
            GlobalObject.Remove(player, 5462, new Vector3(1914.2109, -1300.0547, 30.7266), 0.25f);

            GlobalObject.Remove(player, 17871, new Vector3(2511.7578, -1544.3125, 18.5156), 0.25f);
            GlobalObject.Remove(player, 17509, new Vector3(2511.7578, -1544.3125, 18.5156), 0.25f);

            GlobalObject.Remove(player, 17739, new Vector3(2587.0781, -1589.4453, 15.2734), 0.25f);
            GlobalObject.Remove(player, 17507, new Vector3(2587.0781, -1589.4453, 15.2734), 0.25f);

            GlobalObject.Remove(player, 5679, new Vector3(2286.1719, -974.8047, 30.5000), 0.25f);
            GlobalObject.Remove(player, 5680, new Vector3(2286.1719, -974.8047, 30.5000), 0.25f);

            GlobalObject.Remove(player, 5530, new Vector3(2112.9375, -1797.0938, 19.3359), 0.25f);
            GlobalObject.Remove(player, 5418, new Vector3(2112.9375, -1797.0938, 19.3359), 0.25f);
            GlobalObject.Remove(player, 1522, new Vector3(2105.9219, -1807.2500, 12.5156), 0.25f);

        }

        protected override void OnPlayerSpawned(BasePlayer sender, SpawnEventArgs e)
        {
            Player player = (sender as Player);

            if (!player.IsLogged)
            {
                Console.WriteLine("Spawn without login. How could you do that? :(");
                player.Kick();
                return;
            }

            sender.SendClientMessage("OnPlayerSpawned");
            sender.SendClientMessage(player.MyAccount.LastPosition.IsEmpty.ToString());

            player.Money = 9999;
            base.OnPlayerSpawned(sender, e);
        }

        [Command("dick1")]
        private static void Dick1(BasePlayer sender)
        {
            BaseVehicle v = BaseVehicle.Create(VehicleModelType.CombineHarvester, sender.Position, 0.0f, -1, -1);
        }

        static Item item;

        [Command("dick2")]
        private static void Dick2(BasePlayer sender)
        {
            item = new Item(cornItem, sender.Position);
        }

        [Command("dick3")]
        private static void Dick3(BasePlayer sender)
        {
            item.Give((Player)sender);
        }
        
        [Command("dick4")]
        private static void Dick4(BasePlayer sender)
        {
            Vehicle v = BaseVehicle.Create(VehicleModelType.Bullet, sender.Position, 0.0f, -1, -1) as Vehicle;
            v.Fuel = Common.MAX_VEHICLE_FUEL;
            v.Engine = true;
            v.Lights = true;
        }

        [Command("dick5")]
        private static void Dick5(BasePlayer sender, int interior)
        {
            House h = new House(Interior.GetAll<Interior>()[interior], sender.Position, sender.Angle);
            //h.PutPlayerIn(sender as Player);
        }

        [Command("dick6")]
        private static void Dick6(BasePlayer sender)
        {
            sender.ToggleSpectating(false);
            sender.SetSpawnInfo(-1, 68, Vector3.Zero, 0.0f);
            //sender.Spawn();
        }

        [Command("skin")]
        private static void Skin(BasePlayer sender, int skinid)
        {
            sender.Skin = skinid;
        }


        [Command("kill")]
        private static void Kill(BasePlayer sender)
        {
            sender.Health = 0.0f;
        }

        [Command("dick8")]
        private static void dick8(BasePlayer sender)
        {
            sender.Vehicle.Engine = !sender.Vehicle.Engine;
        }

        [Command("dick9")]
        private static void dick9(BasePlayer sender)
        {
            sender.SendClientMessage((sender as Player).Faction.ToString());
            sender.SendClientMessage((sender as Player).Rank.ToString());
        }

        [Command("dick10")]
        private static void dick10(BasePlayer sender)
        {
            sender.Position = new Vector3(2576.87, -1506.83, 17.6);
        }

        [Command("dick11")]
        private static void dick11(BasePlayer sender)
        {
            Vehicle v = BaseVehicle.Create(VehicleModelType.BMX, sender.Position, 0.0f, -1, -1) as Vehicle;
            v.Fuel = Common.MAX_VEHICLE_FUEL;
            v.Engine = true;
            v.Lights = true;
        }

        //[Command("dick10")]
        //private static void dick10(BasePlayer sender)
        //{
        //    Vehicle v = sender.Vehicle as Vehicle;

        //    if(v != null)
        //    {
        //        Faction.MakeMember((sender as Player).Faction, v);
        //    }
        //}
    }
}
