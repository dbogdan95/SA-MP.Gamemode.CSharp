using Game.Core;
using Game.World.Property;
using Game.World.Property.House;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using System;
using System.Linq;

namespace Game.Account
{
    public partial class Account
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime Birthday { get; private set; }
        public Int16 Gender { get; private set; }
        public Vector3 LastPosition { get; set; }
        public float LastAngle { get; private set; }
        public Property LastProperty { get; set; }

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
            LastPosition = new Vector3();
            LastProperty = null;
        }

        private bool Load(string InputPassword)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM players WHERE name=@name AND password=@pass", conn);
                cmd.Parameters.AddWithValue("@name", __player.Name);
                cmd.Parameters.AddWithValue("@pass", Util.Sha256_hash(InputPassword));
                MySqlDataReader data = cmd.ExecuteReader();

                if (data.HasRows)
                {
                    while (data.Read())
                    {
                        Id                          = (int)data["id"]; 
                        PasswordHash                = (string)data["password"];
                        Email                       = (string)data["email"];
                        Birthday                    = DateTime.Parse((string)data["birthday"]);
                        Gender                      = (Int16)data["gender"];

                        LastPosition                = new Vector3((float)data["x"], (float)data["y"], (float)data["z"]);
                        LastAngle                   = (float)data["a"];

                        int lastprop                = (int)data["property"];
                        LastProperty                = Property.GetPropertyBySQLID(lastprop);
                        if (lastprop > 0 && !(LastProperty is Property))
                        {
                            ClearLastData();
                        }

                        __player.RentedRoom         = Property.GetPropertyBySQLID((int)data["rent"]) as House;
                        __player.House              = Property.GetPropertyBySQLID((int)data["house"]) as House;

                        __player.SendClientMessage("Load("+ Util.Sha256_hash(InputPassword) + ")");
                    }
                    return true;
                }
            }
            return false;
        }

        private void Save()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE players SET x=@x, y=@y, z=@z, a=@a, property=@property, rent=@rent, house=@house WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@x", __player.Position.X);
                cmd.Parameters.AddWithValue("@y", __player.Position.Y);
                cmd.Parameters.AddWithValue("@z", __player.Position.Z);
                cmd.Parameters.AddWithValue("@a", __player.Angle);
                cmd.Parameters.AddWithValue("@property", __player.Property?.GetSqlID());
                cmd.Parameters.AddWithValue("@rent", __player.RentedRoom?.GetSqlID());
                cmd.Parameters.AddWithValue("@house", __player.House?.GetSqlID());
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
                cmd.Parameters.AddWithValue("@gen", Gender);
                cmd.ExecuteNonQuery();

                Id = (int)cmd.LastInsertedId;
            }
        }

        private void Spawn()
        {
            __player.SendClientMessage("Spawn()");
            __player.IsLogged = true;
            __player.SetSpawnInfo(-1, 68, new Vector3(), 0.0f);
            __player.ToggleSpectating(false);
            __player.PutCameraBehindPlayer();
        }

        public static Player GetPlayerBySQLID(int id)
        {
            return Player.GetAll<Player>().Where(p => p.MyAccount.Id == id).FirstOrDefault();
        }

        public static string GetSQLNameFromSQLID(int id)
        {
            if (id == 0)
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
    }
}
