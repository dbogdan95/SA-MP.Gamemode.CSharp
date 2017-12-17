using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System;
using System.Linq;

namespace Game.World.Property.House
{
    public class House : Property
    {
        private int __id;
        private int __level = 1;
        private int __rent = 0;

        public House(Interior interior, Vector3 pos, float angle, int id)
            : base(id, PropertyType.TypeHouse, interior, pos, angle)
        {
            __id = id;
            UpdateLabel();
        }

        public House(Interior interior, Vector3 pos, float angle)
            : base(PropertyType.TypeHouse, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO houses (baseProperty) VALUES ("+Id+")", conn);
                cmd.ExecuteNonQuery();

                __id = (int)cmd.LastInsertedId;
            }
            UpdateLabel();
        }

        //public override bool Locked
        //{
        //    get => base.Locked;
        //    set
        //    {
        //        base.Locked = value;
        //        //Pickup.ModelId = value ? 19522 : (int)Type;
        //    }
        //}

        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE houses SET rent=@rent, level=@level WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@rent", Rent);
                cmd.Parameters.AddWithValue("@level", Level);
                cmd.Parameters.AddWithValue("@id", __id);
                cmd.ExecuteNonQuery();
            }
            base.UpdateSql();
        }

        public override void UpdateLabel()
        {
            string label = null;

            if (Rent > 0 || Price > 0)
                label += "[House - Lvl: " + Level + "]\n\r\n\r";
            else
                label += "[House - Lvl: " + Level + "]\n\r";

            if (Owner != 0)
            {
                label += "Owner: " + Account.Account.GetSQLNameFromSQLID(Owner) + "\n\r";
            }

            if (Rent > 0)
            {
                label += "For rent: " + Util.FormatNumber(Rent) + " per hour\n\r";
                label += "Use /rent to rent this house\n\r\n\r";
            }

            if (Price > 0)
            {
                label += "For sell: " + Util.FormatNumber(Price) + "\n\r";
                label += "Use /buy to buy this house\n\r\n\r";
            }
            Label.Text = label;
        }
        public int Rent
        {
            get => __rent;
            set
            {
                if (__rent > 0 && value == 0)
                {
                    foreach (Player p in Player.GetAll<Player>().Where(p => p.RentedRoom == this))
                        p.RentedRoom = null;

                    using (var conn = Database.Connect())
                    {
                        new MySqlCommand("UPDATE players SET rent = NULL WHERE rent="+Id, conn)
                            .ExecuteNonQuery();
                    }
                }

                __rent = value;
            }
        }
        public int Level
        {
            get => __level;
            set => __level = value;
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
                    lastOwner.House = null; // Ii anulam propietatea

                // II in baza de date
                using (var conn = Database.Connect())
                {
                    new MySqlCommand("UPDATE players SET house = NULL WHERE id="+Owner, conn)
                        .ExecuteNonQuery();
                }
            }

            if (ownerSqlID != 0)
            {
                Player newOnwer = Account.Account.GetPlayerBySQLID(ownerSqlID);

                if (newOnwer is Player) // Verificam daca noul owner este conectat
                {
                    if (newOnwer.House is House) // Verificam daca noul owner a avut si el la randul lui o casa
                        newOnwer.House.PutToSell(); // Scoatem casa la vanzare;

                    newOnwer.House = this; // Ii atribuim noua casa
                    newOnwer.RentedRoom = null; // II anulam rentul, in caz ca avea
                }
                else // Ownerul nou nu este in joc.
                {
                    using (var conn = Database.Connect())
                    {
                        // Selectam casa din baza de date
                        MySqlCommand cmd = new MySqlCommand("SELECT house FROM players WHERE id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", ownerSqlID);
                        var oldhouse = cmd.ExecuteScalar();

                        if (!(oldhouse is DBNull))
                        {
                            // Jucatorul offline are o casa
                            if (Find((int)oldhouse) is House oldHouse)
                                oldHouse.PutToSell();
                        }
                    }
                }

                using (var conn = Database.Connect())
                {
                    // II actualizam noua casa in baza de date
                    MySqlCommand cmd = new MySqlCommand("UPDATE players SET house=@house, rent=NUll WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@house", Id);
                    cmd.Parameters.AddWithValue("@id", ownerSqlID);
                    cmd.ExecuteNonQuery();
                }
            }
            else
                PutToSell();

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
                MySqlCommand cmd = new MySqlCommand("UPDATE players SET rent = NULL WHERE rent=@id", conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }

            Owner = 0;

            Locked = false;
            Price = (int)(Level * Common.HOUSE_BASE_COST);
            UpdateLabel();
            UpdateSql();
        }
    }
}
