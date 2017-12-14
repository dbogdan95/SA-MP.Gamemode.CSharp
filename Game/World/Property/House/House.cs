using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System.Linq;

namespace Game.World.Property.House
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
        public override int GetSqlID()
        {
            return (int)__SQLID;
        }
        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE houses SET interior=@interior, locked=@locked, deposit=@deposit, rent=@rent, level=@level, owner=@owner WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.Index(Interior));
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.Parameters.AddWithValue("@deposit", Deposit);
                cmd.Parameters.AddWithValue("@rent", Rent);
                cmd.Parameters.AddWithValue("@level", Level);
                cmd.Parameters.AddWithValue("@owner", Owner);
                cmd.Parameters.AddWithValue("@id", __SQLID);
                cmd.ExecuteNonQuery();
            }
        }
        public override void UpdateLabel()
        {
            string label = null;

            label += "[House - Lvl: " + Level + "]\n\r";

            if(Owner != 0)
            {
                label += "Owner: " + Account.Account.GetSQLNameFromSQLID(Owner) + "\n\r";
            }

            if (Rent > 0)
            {
                label += "For rent: " + Util.FormatNumber(Rent) + " per hour\n\r";
                label += "Use /rentroom to rent this house\n\r";
            }

            if (Price > 0)
            {
                label += "For sell: " + Util.FormatNumber(Price) + "\n\r";
                label += "Use /buy to buy this house\n\r";
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
        public override int Price
        {
            get => base.Price;
            set
            {
                base.Price = value;
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
        public override void SetOwnerUpdate(int ownerSqlID)
        {
            if (Owner == ownerSqlID)
                return;

            if (ownerSqlID < 0)
                ownerSqlID = 0;

            // Deja are un owner
            if (Owner != 0)
            {
                Player lastOwner = Account.Account.GetPlayerBySQLID(Owner);

                // Verificam daca fostul owner se afla in joc
                if (lastOwner is Player)
                {
                    lastOwner.House = null; // Ii anulam propietatea
                }

                // II in baza de date
                using (var conn = Database.Connect())
                {
                    MySqlCommand cmd = new MySqlCommand("UPDATE players SET house = 0 WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", Owner);
                    cmd.ExecuteNonQuery();
                }
            }

            if(ownerSqlID != 0)
            {
                Player newOnwer = Account.Account.GetPlayerBySQLID(ownerSqlID);

                if (newOnwer is Player) // Verificam daca noul owner este conectat
                {
                    if (newOnwer.House is House) // Verificam daca noul owner a avut si el la randul lui o casa
                    {
                        newOnwer.House.PutToSell(); // Scoatem casa la vanzare;
                    }

                    newOnwer.House = this; // Ii atribuim noua casa
                }
                else // Ownerul vechi nu este in joc.
                {
                    using (var conn = Database.Connect())
                    {
                        // Selectam casa din baza de date
                        MySqlCommand cmd = new MySqlCommand("SELECT house FROM players WHERE id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", ownerSqlID);
                        int oldhouse = (int)cmd.ExecuteScalar();

                        // Jucatorul offline are o casa
                        if (GetPropertyBySQLID(oldhouse) is House oldHouse)
                            oldHouse.PutToSell();
                    }
                }

                using (var conn = Database.Connect())
                {
                    // II actualizam noua casa in baza de date
                    MySqlCommand cmd = new MySqlCommand("UPDATE players SET house = @house WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@house", GetSqlID());
                    cmd.Parameters.AddWithValue("@id", ownerSqlID);
                    cmd.ExecuteNonQuery();
                }
            }

            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE houses SET owner = @owner WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@owner", ownerSqlID);
                cmd.Parameters.AddWithValue("@id", GetSqlID());
                cmd.ExecuteNonQuery();
            }
            Owner = ownerSqlID;
        }

        public void PutToSell()
        {
            foreach (Player player in Player.GetAll<Player>().Where(p => p.RentedRoom == this))
            {
                player.RentedRoom = null;
                player.SendClientMessage("** You were unexpectedly evacuated from your rent.");
            }
                
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE players SET rent = 0 WHERE rent=@id", conn);
                cmd.Parameters.AddWithValue("@id", GetSqlID());
                cmd.ExecuteNonQuery();
            }

            Owner = 0;

            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE houses SET owner = 0 WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@id", GetSqlID());
                cmd.ExecuteNonQuery();
            }

            Price = (int)(Level * Common.HOUSE_BASE_COST);
            UpdateLabel();
        }
    }
}
