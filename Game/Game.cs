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
using Game.World.Item;
using Game.World.PaynSpray;
using Game.World.Property;
using Game.Controllers;
using Game.Core;

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

            base.OnInitialized(e);
            Database.Connect();

            SetGameModeText("cioak");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            ManualVehicleEngineAndLights();

            Item item = new Item(cornItem, new Vector3());

            new PaynSpray(-1002, new Vector3(1024.9884, -1029.3324, 35.1272), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(1024.9884, -1029.3324, 33.1772), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(1021.8430, -1018.5289, 31.1826), new Vector3(1027.9781, -1029.1014, 36.8535), new Vector3(1024.6614, -1035.0811, 34.7631)).Open();
            new PaynSpray(-1003, new Vector3(488.2576, -1734.8486, 14.6693), new Vector3(0.0000, 90.0000, -97.9200), new Vector3(488.2576, -1734.8486, 12.3906), new Vector3(0.0000, 0.0000, -97.9200), new Vector3(489.7542, -1747.4993, 10.2083), new Vector3(484.9799, -1734.7427, 16.7146), new Vector3(489.6070, -1728.6317, 14.9308)).Open();
            new PaynSpray(-1004, new Vector3(-1420.5447, 2591.2354, 60.5496), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-1420.5447, 2591.2354, 57.7422), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-1416.1711, 2590.9380, 54.8367), new Vector3(-1424.9185, 2576.6777, 60.2768), new Vector3(-1419.8292, 2598.4800, 60.3985));
            new PaynSpray(-1005, new Vector3(-1904.5099, 277.6387, 45.7687), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-1904.5099, 277.6387, 42.9531), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-1900.1495, 292.2030, 40.0500), new Vector3(-1908.8933, 278.7156, 45.5111), new Vector3(-1905.0630, 270.2467, 46.0506));
            new PaynSpray(-1006, new Vector3(-2425.6450, 1028.2318, 55.0539), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-2425.6450, 1028.2318, 52.2813), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-2430.0173, 1013.7064, 49.3985), new Vector3(-2421.3354, 1027.4117, 54.8807), new Vector3(-2425.1062, 1035.7886, 55.6979));
            new PaynSpray(-1007, new Vector3(1968.4006, 2162.5015, 14.0288), new Vector3(0.0000, 90.0000, 0.0000), new Vector3(1968.4006, 2162.5015, 12.0938), new Vector3(), new Vector3(1983.1675, 2166.8013, 9.9969), new Vector3(1968.7124, 2158.6880, 15.6400), new Vector3(1962.0724, 2162.7275, 14.1024));
            new PaynSpray(-1002, new Vector3(2071.5762, -1831.4224, 16.4406), new Vector3(0.0000, 90.0000, 0.0000), new Vector3(2071.5564, -1831.4203, 14.5625), new Vector3(), new Vector3(2056.6753, -1835.9222, 12.4891), new Vector3(2070.4165, -1827.1287, 18.0190), new Vector3(2076.6843, -1831.5831, 16.5109)).Open();
            new PaynSpray(-1004, new Vector3(-100.0399, 1111.4144, 24.4802), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(-100.0399, 1111.4144, 21.6406), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(-104.4104, 1125.9390, 18.6544), new Vector3(-95.6366, 1112.2601, 24.2028), new Vector3(-100.8332, 1103.6481, 23.6690));
            new PaynSpray(-1008, new Vector3(720.0145, -462.4834, 18.6588), new Vector3(0.0000, 90.0000, 90.0000), new Vector3(720.0145, -462.4834, 16.8594), new Vector3(0.0000, 0.0000, 90.0000), new Vector3(715.7748, -447.3318, 14.9643), new Vector3(724.2939, -461.0360, 20.4023), new Vector3(719.7719, -467.8614, 18.5736)).Open();

            //House h = new House(Interior.GetAll<Interior>()[3], new Vector3(1023.1342, -1031.1912, 31.9978), 1.0f);
           // Console.WriteLine(h.ToString());
        }

        protected override void LoadControllers(ControllerCollection controllers)
        {
            base.LoadControllers(controllers);
            controllers.Add(new PlayerController());
            controllers.Add(new VehicleController());
            controllers.Add(new CornPlantController());
            controllers.Add(new ClockController());
            controllers.Add(new ItemController());
            controllers.Add(new InteriorController());
            controllers.Add(new PropertyController());
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
        }

        protected override void OnPlayerSpawned(BasePlayer player, SpawnEventArgs e)
        {
            base.OnPlayerSpawned(player, e);
            player.Position = new Vector3(1023.1342, -1031.1912, 31.9978);
            
            player.Money = 9999;
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
            BaseVehicle v = BaseVehicle.Create(VehicleModelType.Bullet, sender.Position, 0.0f, -1, -1);
            v.Engine = true;
            v.Lights = true;
        }

        [Command("dick5")]
        private static void Dick5(BasePlayer sender, int interior)
        {
            House h = new House(Interior.GetAll<Interior>()[interior], sender.Position, sender.Angle);
            //h.PutPlayerIn(sender as Player);
        }

        [Command("skin")]
        private static void Skin(BasePlayer sender, int skinid)
        {
            sender.Skin = skinid;
        }
    }
}
