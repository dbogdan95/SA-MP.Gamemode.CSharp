using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;

namespace Game.World.Property
{
    public class Business : Property
    {
        private long __SQLID;

        public Business(Interior interior, Vector3 pos, float angle) 
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO business (interior, x, y, z, a, locked) VALUES (@interior, @x, @y, @z, @a, @locked)", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.GetAll<Interior>().IndexOf(interior));
                cmd.Parameters.AddWithValue("@x", pos.X);
                cmd.Parameters.AddWithValue("@y", pos.Y);
                cmd.Parameters.AddWithValue("@z", pos.Z);
                cmd.Parameters.AddWithValue("@a", angle);
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.ExecuteNonQuery();

                __SQLID = cmd.LastInsertedId;
            }
        }
        public Business(Interior interior, Vector3 pos, float angle, int sqlid)
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            __SQLID = sqlid;
        }
        public override void Remove()
        {
            System.Console.WriteLine("Remove - from business");
            using (var conn = Database.Connect())
            {
                new MySqlCommand("DELETE FROM business WHERE id=" + __SQLID, conn)
                    .ExecuteNonQuery();
            }
            base.Remove();
        }

        public override void UpdateLabel()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE business SET interior=@interior, locked=@locked, deposit=@deposit WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.Index(Interior));
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.Parameters.AddWithValue("@deposit", Deposit);
                cmd.Parameters.AddWithValue("@id", __SQLID);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
