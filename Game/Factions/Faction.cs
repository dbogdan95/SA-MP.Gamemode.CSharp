using Game.Core;
using Game.World.Players;
using Game.World.Properties;
using Game.World.Vehicles;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Game.Factions
{
    public partial class Faction : IdentifiedPool<Faction>
    {
        private string __name;
        private Color __color;
        private FactionCategory __type;

        private readonly Dictionary<int, Rank> __ranks = new Dictionary<int, Rank>();

        private Faction(int id, FactionCategory type, Color color, string name)
        {
            __Faction(id, type, color, name);
        }

        public ReadOnlyDictionary<int, Rank> GetRanks { get => new ReadOnlyDictionary<int, Rank>(__ranks.ToDictionary(k => k.Key, v => v.Value)); }

        public IEnumerable<Player> PlayersInFaction(Faction faction)
        {
            return Player.GetAll<Player>().ToArray().Where(pl => pl.Faction == faction);
        }

        public IEnumerable<Vehicle> VehiclesInFaction(Faction faction)
        {
            return Vehicle.GetAll<Vehicle>().ToArray().Where(veh => veh.Faction == faction);
        }

        public Generic Headquarter => Property.GetAll<Generic>().Where(generic => generic.Faction == this).FirstOrDefault();

        public string Name { get => __name; set => __name = value; }
        public Color Color { get => __color; set => __color = value; }
        public FactionCategory Category { get => __type; }

        public bool Invite(Player player/*, bool updateDB = false*/)
        {
            if (player.Faction != null)
                throw new Exception(player.ToString() + " is already in a faction.");

            player.Faction = this;
            player.Rank = 1;
            player.Skin = __ranks[1].Skin;
            player.Color = __color;

            /*if (updateDB)*/
            __insertOrUpdateMember(player);

            return true;
        }

        public bool Invite(Vehicle vehicle/*, bool updateDB = false*/)
        {
            if (vehicle.Faction != null)
                throw new Exception(vehicle.ToString() + " is already in a faction.");

            vehicle.Faction = this;
            vehicle.Rank = 1;
            /*if (updateDB)*/
            __insertOrUpdateMember(vehicle);

            return true;
        }

        public bool Dismiss(Player player/*, bool updateDB = false*/)
        {
            if (player.Faction == null)
                throw new Exception(player.ToString() + " is not in a faction.");

            player.Faction = null;
            player.Rank = null;
            player.Color = Color.White;

            /*if (updateDB)*/
            __deleteMember(player);

            return true;
        }

        public bool Dismiss(Vehicle vehicle/*, bool updateDB = false*/)
        {
            if (vehicle.Faction == null)
                throw new Exception(vehicle.ToString() + " is not in a faction.");

            vehicle.Faction = null;
            vehicle.Rank = null;
            /*if (updateDB)*/
            __deleteMember(vehicle);

            return true;
        }

        public bool SetRank(Player player, int? rank/*, bool updateDB = false*/)
        {
            if (player.Faction == null)
                throw new Exception(player.ToString() + " is not in a faction.");

            if (!ValidRankid(rank))
                throw new IndexOutOfRangeException("Rank " + rank + " out of bonds for " + player.Faction.ToString());

            player.Rank = rank;

            if (player.MyAccount.Gender == Accounts.GenderType.GenderFemale)
                player.Skin = __ranks[0].Skin;  // Fake rank. Only one skin for girls.. (#NotMisogynist).
            else
                player.Skin = __ranks[rank.Value].Skin;

            /*if (updateDB)*/
            __insertOrUpdateMember(player);

            return true;
        }

        public bool SetRank(Vehicle vehicle, int? rank/*, bool updateDB = false*/)
        {
            if (vehicle.Faction == null)
                throw new Exception(vehicle.ToString() + " is not in a faction.");

            if (!__ranks.ContainsKey(rank.Value))
                throw new IndexOutOfRangeException("Rank " + rank + " out of bonds for " + vehicle.Faction.ToString());

            vehicle.Rank = rank;

            /*if (updateDB)*/
            __insertOrUpdateMember(vehicle);

            return true;
        }

        public int Skin(int? rank)
        {
            if (!__ranks.ContainsKey(rank.Value))
                return 0;

            return __ranks[rank.Value].Skin;
        }

        public override string ToString()
        {
            return "Faction(Id: " + Id + ", Name: " + Name + ")";
        }

        public bool ValidRankid(int? rankid)
        {
            return (rankid != null && rankid > 0 && __ranks.ContainsKey(rankid.Value));
        }

        public void SendMessage(string str)
        {
            foreach (Player player in Player.GetAll<Player>().Where(p => p.Faction == this))
            {
                player.SendClientMessage(Color.Cyan, str);
            }
        }

        public void SendMessage(Color color, string str)
        {
            foreach (Player player in Player.GetAll<Player>().Where(p => p.Faction == this))
            {
                player.SendClientMessage(color, str);
            }
        }

        public static void Load()
        {
            int facts = 0;
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM factions", conn);
                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    new Faction(data.GetInt32("id"), (FactionCategory)data.GetInt32("type"), data.GetInt32("color"), data.GetString("name"));
                    facts++;
                }
                data.Close();

                foreach(Faction faction in GetAll<Faction>())
                {
                    cmd = new MySqlCommand("SELECT * FROM ranks WHERE faction=" + faction.Id, conn);
                    data = cmd.ExecuteReader();

                    while (data.Read())
                    {
                        faction.__addRanks(data.GetInt32("rank"), new Rank(data.GetString("name"), data.GetInt32("skin")));
                    }
                    data.Close();

                    cmd = new MySqlCommand("SELECT A.id, model, x, y, z, a, color1, color2, siren, B.rank " +
                        "FROM vehicles as A " +
                        "INNER JOIN vehicles_factions as B " +
                        "ON A.id = B.baseVehicle " +
                        "WHERE B.baseFaction =" + faction.Id, conn);

                    data = cmd.ExecuteReader();

                    while (data.Read())
                    {
                        Vehicle v = BaseVehicle.Create((VehicleModelType)data.GetInt32("model"), 
                            new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), 
                            data.GetFloat("a"), data.GetInt32("color1"), data.GetInt32("color2"), 
                            1800, data.GetBoolean("siren")) as Vehicle;

                        v.SQLID = data.GetInt32("id");
                        v.Faction = faction;
                        v.Rank = data.GetInt32("rank");
                    }
                    data.Close();
                }
            }
            Console.WriteLine("** Loaded {0} factions from database.", facts);
        }
    }
}
