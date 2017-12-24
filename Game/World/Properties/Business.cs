using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using System.Linq;
using System;
using Game.World.Players;
using Game.Accounts;
using SampSharp.Streamer.World;

namespace Game.World.Properties
{
    public class Business : Property
    {
        private int __id;
        private BusinessType __type;
        private int __domainid;
        private DynamicMapIcon __icon = null;

        public Business(Interior interior, Vector3 pos, float angle) 
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO business (baseProperty) VALUES (" + Id + ")", conn);
                cmd.ExecuteNonQuery();

                __id = (int)cmd.LastInsertedId;
            }
            BizzType = null;
        }

        private Business(Interior interior, Vector3 pos, float angle, int id, int baseid)
            : base(baseid, PropertyType.TypeBusiness, interior, pos, angle)
        {
            __id = id;
            BizzType = null;
        }

        public BusinessType BizzType
        {
            get => __type;
            set
            {
                __type = value;

                if(__type != null)
                {
                    if (__type.Icon < 0 || __type.Icon > 63)
                    {
                        if (__icon != null)
                            __icon.Dispose();
                    }
                    else
                    {
                        if (__icon != null)
                            __icon.Type = __type.Icon;
                        else
                            __icon = new DynamicMapIcon(Position, __type.Icon);
                    }
                }
                UpdateLabel();
            }
        }

        public int Domainid
        {
            get => __domainid;
            set => __domainid = value;
        }

        public override void UpdateLabel()
        {
            string label = string.Empty;

            if (Price > 0)
            {
                if (__type != null)
                    label += "[Business - " + __type.ToString() + "]\n\r\n\r";
                else
                    label += "[Business]\n\r\n\r";
            }
            else
            {
                if (__type != null)
                    label += "[Business - " + __type.ToString() + "]\n\r";
                else
                    label += "[Business]\n\r";
            }

            if(Owner != null)
            {
                label += "Owner: " + Account.GetSQLNameFromSQLID(Owner) + "\n\r";
            }

            if (Price > 0)
            {
                label += "For sell: " + Util.FormatNumber(Price) + "\n\r";
                label += "Use /buy to buy this business\n\r";
            }

            Label.Text = label;
        }

        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE business SET type=@type, domainid=@domainid WHERE id=@id", conn);

                if(BizzType is null)
                    cmd.Parameters.AddWithValue("@type", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@type", (int)BizzType.Type);

                cmd.Parameters.AddWithValue("@domainid", Domainid);
                cmd.Parameters.AddWithValue("@id", __id);
                cmd.ExecuteNonQuery();
            }
            base.UpdateSql();
        }

        public static Business FindByDomain(int domainid, BusinessTypes type)
        {
            return GetAll<Business>().Where(b => b.BizzType == BusinessType.FromType(type) && b.Domainid == domainid)
                .FirstOrDefault();
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
                Player lastOwner = Account.GetPlayerBySQLID(Owner);

                // Verificam daca fostul owner se afla in joc
                if (lastOwner is Player)
                    lastOwner.Business = null; // Ii anulam propietatea

                // II in baza de date
                using (var conn = Database.Connect())
                {
                    new MySqlCommand("UPDATE players SET business = NULL WHERE id="+Owner, conn)
                        .ExecuteNonQuery();
                }
            }

            if (ownerSqlID != 0)
            {
                Player newOnwer = Account.GetPlayerBySQLID(ownerSqlID);

                if (newOnwer is Player) // Verificam daca noul owner este conectat
                {
                    if (newOnwer.Business is Business) // Verificam daca noul owner a avut si el la randul lui o casa
                        newOnwer.Business.PutToSell(); // Scoatem casa la vanzare;

                    newOnwer.Business = this; // Ii atribuim noua casa
                }
                else // Ownerul vechi nu este in joc.
                {
                    using (var conn = Database.Connect())
                    {
                        // Selectam casa din baza de date
                        MySqlCommand cmd = new MySqlCommand("SELECT business FROM players WHERE id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", ownerSqlID);
                        var oldbusiness = cmd.ExecuteScalar();

                        if (!(oldbusiness is DBNull))
                        {
                            // Jucatorul offline are o casa
                            if (Find((int)oldbusiness) is Business prop)
                                prop.PutToSell();
                        }
                    }
                }

                using (var conn = Database.Connect())
                {
                    // II actualizam noua casa in baza de date
                    MySqlCommand cmd = new MySqlCommand("UPDATE players SET business = @business WHERE id=@id", conn);
                    cmd.Parameters.AddWithValue("@business", Id);
                    cmd.Parameters.AddWithValue("@id", ownerSqlID);
                    cmd.ExecuteNonQuery();
                }
            }
            else
                PutToSell();

            Owner = ownerSqlID;
        }

        private void PutToSell()
        {
            Owner = 0;
            Locked = false;
            Price = BizzType.Price;
            UpdateLabel();
            UpdateSql();
        }

        public static void Load()
        {
            int props = 0;
            using (var conn = Database.Connect())
            {
                    MySqlCommand cmd = new MySqlCommand("SELECT A.*, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior, C.price, B.id AS owner FROM business as A  " +
                        "INNER JOIN properties AS C ON c.id = A.baseProperty " +
                        "LEFT JOIN players as B ON C.id = B.business ", conn);

                var data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Business b = new Business(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"), data.GetInt32("baseProperty"))
                    {
                        Locked = data.GetBoolean("locked"),
                        Deposit = data.GetInt32("deposit"),
                        BizzType = data["type"] is DBNull ? null : BusinessType.FromType((BusinessTypes)data.GetInt32("type")),
                        Domainid = data.GetInt32("domainid"),
                        Price = data.GetInt32("price")
                    };

                    if (data["owner"] is DBNull)
                        b.Owner = null;
                    else
                        b.Owner = data.GetInt32("owner");
                    
                    b.UpdateLabel();
                    props++;
                }
                data.Close();
            }
            Console.WriteLine("** Loaded {0} businesses from database.", props);
        }
    }
}
