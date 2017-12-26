﻿using System;
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
using Game.Core;
using Game.Display;
using Game.World.Items;
using Game.World.Properties;
using Game.World.GasStations;
using Game.World.Players;
using Game.World.PaynSprays;
using Game.Accounts;
using Game.World.Vehicles;
using Game.World.Zones;
using SampSharp.Streamer.World;
using SampSharp.GameMode.SAMP;
using Game.Factions;

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
            ForeColor = SampSharp.GameMode.SAMP.Color.Silver,
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
            //new GasStation(5, new Vector3(-2405.5562, 958.0877, 44.2969), new Vector3(-2416.3118, 992.0084, 50.2754)).Off(); 
            new GasStation(6, new Vector3(-2251.0879, -2562.7524, 30.9237), new Vector3(-2237.7690, -2559.9014, 36.2992));
            //new GasStation(7, new Vector3(-1659.4701, 417.4124, 6.1868), new Vector3(-1695.6837, 412.9560, 10.5386)).Off();
            //new GasStation(8, new Vector3(2191.8389, 2467.4988, 9.8208), new Vector3(2211.9209, 2482.1394, 14.6517)).Off();
            //new GasStation(9, new Vector3(629.7487, 1678.8574, 5.9884), new Vector3(598.1221, 1705.9727, 10.9382)).Off();
            //new GasStation(10, new Vector3(2122.1111, 910.3763, 9.7593), new Vector3(2107.5552, 929.7073, 14.6152)).Off();
            //new GasStation(11, new Vector3(2647.6121, 1117.0409, 9.8205), new Vector3(2632.7041, 1096.7050, 14.6148)).Off();
            //new GasStation(12, new Vector3(2155.4351, 2737.9014, 9.8211), new Vector3(2140.4011, 2757.5012, 14.6144)).Off();
            //new GasStation(13, new Vector3(1606.2572, 2188.6067, 9.8207), new Vector3(1586.8804, 2208.7385, 14.6514)).Off();
            //new GasStation(14, new Vector3(-1463.0481, 1858.7996, 31.6321), new Vector3(-1479.0486, 1869.0215, 36.8829)).Off();
            new GasStation(15, new Vector3(650.8358, -576.4760, 15.3088), new Vector3(660.3215, -553.9163, 20.5575));
            new GasStation(16, new Vector3(1386.9375, 454.7686, 18.9276), new Vector3(1378.8147, 467.7030, 22.1437));

            new DynamicObject(-1010, new Vector3(1571.6016, -1675.7500, 35.6677), Vector3.Zero);
            new DynamicObject(-1011, new Vector3(1571.3461, -1670.9417, 17.6223), Vector3.Zero);

            new DynamicObject(-1012, new Vector3(1555.8922, -1674.0765, 15.1955), new Vector3(0.0000, 0.0000, -90.0000));
            new DynamicObject(-1012, new Vector3(1555.9153, -1677.1036, 15.1955), new Vector3(0.0000, 0.0000, 90.0000));

            Zone.Load(@"zones.xml");
            Interior.Load(@"interiors.xml");
            House.Load();
            Business.Load();
            Faction.Load();
            Generic.Load();

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
            GlobalObject.Remove(player, 1536, new Vector3(1555.9297, -1677.1250, 15.1797), 0.25f);
            GlobalObject.Remove(player, 1536, new Vector3(1555.8906, -1674.1094, 15.1797), 0.25f);
            GlobalObject.Remove(player, 3976, new Vector3(1571.6016, -1675.7500, 35.6797), 0.25f);
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
