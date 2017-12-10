using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;

namespace Game.World.Property
{
    public class House : Property
    {
        private long __SQLID;
        private int __level = 1;
        private int __rent = 0;

        public House(Interior interior, Vector3 pos, float angle, long sqlid)
            : base(PropertyType.TypeHouse, interior, pos, angle)
        {
            __SQLID = sqlid;
            UpdateLabel();
        }
        public House(Interior interior, Vector3 pos, float angle)
            : base(PropertyType.TypeHouse, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO houses (interior, x, y, z, a, locked) VALUES (@interior, @x, @y, @z, @a, @locked)", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.GetAll<Interior>().IndexOf(interior));
                cmd.Parameters.AddWithValue("@x", pos.X);
                cmd.Parameters.AddWithValue("@y", pos.Y);
                cmd.Parameters.AddWithValue("@z", pos.Z);
                cmd.Parameters.AddWithValue("@a", angle);
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.ExecuteNonQuery();
                
                __SQLID = cmd.LastInsertedId;
            }
            UpdateLabel();
        }
        public override void Remove()
        {
            // TODO: fix the server freezeing
            System.Console.WriteLine("Remove - from house");
            using (var conn = Database.Connect())
            {
                new MySqlCommand("DELETE FROM houses WHERE id=" + __SQLID, conn)
                    .ExecuteNonQuery();
            }
            base.Remove();
        }
        public override bool Locked
        {
            get => base.Locked;
            set
            {
                base.Locked = value;
                //Pickup.ModelId = value ? 19522 : (int)Type;
            }
        }

        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE houses SET interior=@interior, locked=@locked, deposit=@deposit, rent=@rent, level=@level WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.Index(Interior));
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.Parameters.AddWithValue("@deposit", Deposit);
                cmd.Parameters.AddWithValue("@rent", Rent);
                cmd.Parameters.AddWithValue("@level", Level);
                cmd.Parameters.AddWithValue("@id", __SQLID);
                cmd.ExecuteNonQuery();
            }
        }
        public override void UpdateLabel()
        {
            string label = null;

            label += "[House - Lvl: " + Level + "]\n\r";

            if(Rent > 0)
            {
                label += "For rent: " + Util.FormatNumber(Rent) + " per hour\n\r";
                label += "Use /rentroom to rent this house\n\r";
            }

            Label.Text = label;
        }

        public int Rent
        {
            get => __rent;
            set
            {
                __rent = value;
                UpdateLabel();
            }
        }
        public int Level
        {
            get => __level;
            set
            {
                __level = value;
                UpdateLabel();
            }
        }
    }
}
