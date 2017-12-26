using Game.Core;
using Game.Factions;
using Game.World.Players;
using Game.World.Properties;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using System;
using System.Linq;

namespace Game.Accounts
{
    public partial class Account : IdentifiedPool<Account>
    {
        //public int Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime Birthday { get; private set; }
        public GenderType Gender { get; private set; }
        public Vector3 LastPosition { get; private set; }
        public float LastAngle { get; private set; }
        public Property LastProperty { get; private set; }
        public DateTime RegisterDate { get; private set; }

        private Player __player;

        public Account(Player player)
        {
            __player = player;
            player.ToggleSpectating(true);
            player.VirtualWorld = player.Id;
            player.Disconnected += Player_Disconnected;

            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM players WHERE name=@name", conn);
                cmd.Parameters.AddWithValue("@name", player.Name);
                long count = (long)cmd.ExecuteScalar();

                if (count > 0)
                    Login();
                else
                    Register();
            }
        }

        public void ClearLastData()
        {
            LastAngle = 0f;
            LastPosition = Vector3.Zero;
            LastProperty = null;
        }

        private bool Load(string InputPassword)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT A.*, B.baseFaction as faction, B.rank as rank FROM players AS A " +
                    "LEFT JOIN players_factions AS B " +
                    "ON A.id = B.basePlayer " +
                    "WHERE A.name = @name AND A.password = @pass", conn);

                cmd.Parameters.AddWithValue("@name", __player.Name);
                cmd.Parameters.AddWithValue("@pass", Util.Sha256_hash(InputPassword));
                MySqlDataReader data = cmd.ExecuteReader();

                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        Id                          = data.GetInt32("id"); 
                        PasswordHash                = data.GetString("password");
                        Email                       = data.GetString("email");
                        Birthday                    = DateTime.Parse(data.GetString("birthday"));
                        Gender                      = (GenderType)data.GetInt16("gender");
                        RegisterDate                = Convert.ToDateTime(data.GetString("registeredAt"));

                        LastPosition                = new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z"));
                        LastAngle                   = data.GetFloat("a");
                        LastProperty                = data["property"] is DBNull ? null : Property.Find(data.GetInt32("property"));

                        __player.RentedRoom         = data["rent"] is DBNull ? null : Property.Find(data.GetInt32("rent")) as House;
                        __player.House              = data["house"] is DBNull ? null :  Property.Find(data.GetInt32("house")) as House;
                        __player.Business           = data["business"] is DBNull ? null : Property.Find(data.GetInt32("business")) as Business;

                        if (data["faction"] is DBNull)
                        {
                            __player.Faction = null;
                            __player.Rank = null;
                        }
                        else
                        {
                            __player.Faction = Faction.Find(data.GetInt32("faction"));
                            __player.Rank = data.GetInt32("rank");
                        }
                        
                        __player.SendClientMessage("Load("+ Util.Sha256_hash(InputPassword) + ")");
                    }
                    return true;
                }
            }
            return false;
        }

        public void Save()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE players SET x=@x, y=@y, z=@z, a=@a, property=@property, rent=@rent, house=@house, business=@business WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@x", __player.Position.X);
                cmd.Parameters.AddWithValue("@y", __player.Position.Y);
                cmd.Parameters.AddWithValue("@z", __player.Position.Z);
                cmd.Parameters.AddWithValue("@a", __player.Angle);

                cmd.Parameters.Add(new MySqlParameter("@property", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@rent", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@house", MySqlDbType.Int32));
                cmd.Parameters.Add(new MySqlParameter("@business", MySqlDbType.Int32));

                if (__player.Property == null)
                    cmd.Parameters["@property"].Value = DBNull.Value;
                else
                    cmd.Parameters["@property"].Value = __player.Property.Id;

                if (__player.RentedRoom == null)
                    cmd.Parameters["@rent"].Value = DBNull.Value;
                else
                    cmd.Parameters["@rent"].Value = __player.RentedRoom.Id;

                if (__player.House == null)
                    cmd.Parameters["@house"].Value = DBNull.Value;
                else
                    cmd.Parameters["@house"].Value = __player.House.Id;

                if (__player.Business == null)
                    cmd.Parameters["@business"].Value = DBNull.Value;
                else
                    cmd.Parameters["@business"].Value = __player.Business.Id;

                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }
        }

        private void Insert()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO players (name, password, email, birthday, gender) VALUES (@name, @pass, @email, @birthday, @gen)", conn);
                cmd.Parameters.AddWithValue("@name", __player.Name);
                cmd.Parameters.AddWithValue("@pass", PasswordHash);
                cmd.Parameters.AddWithValue("@email", Email);
                cmd.Parameters.AddWithValue("@birthday", Birthday.ToString("MM/dd/yyyy"));
                cmd.Parameters.AddWithValue("@gen", (short)Gender);
                cmd.ExecuteNonQuery();

                RegisterDate = DateTime.Now;
                Id = (int)cmd.LastInsertedId;
            }
        }

        private void Spawn()
        {
            __player.SendClientMessage("Spawn()");
            __player.IsLogged = true;
            __player.ToggleSpectating(false);
            __player.VirtualWorld = 0;
        }

        public static Player GetPlayerBySQLID(int? id)
        {
            if (id is null || id == 0)
                return null;

            return Player.GetAll<Player>().ToArray().Where(p => p.MyAccount.Id == id).FirstOrDefault();
        }

        public static string GetSQLNameFromSQLID(int? id)
        {
            if (id == null || id == 0)
                return string.Empty;

            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT name FROM players WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                MySqlDataReader data = cmd.ExecuteReader();

                while(data.Read())
                {
                    return data.GetString(0);
                }
            }
            return string.Empty;
        }

        public static bool IsSQLIDValid(int id)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM players WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                return ((long)cmd.ExecuteScalar() > 0);
            }
        }

        public override string ToString()
        {
            return "Account(Id: " + Id + ", User: " + __player.Name + "(" + __player.Id + ")" + ", " + RegisterDate.ToString() + ")";
        }
    }
}
