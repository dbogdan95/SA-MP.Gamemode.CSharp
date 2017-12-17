using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System.Linq;
using System;

namespace Game.World.Property.Business
{
    public class Business : Property
    {
        private int __id;
        private BusinessType __type;
        private int __domainid;
        
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
        public Business(Interior interior, Vector3 pos, float angle, int sqlid)
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            __id = sqlid;
            BizzType = null;
        }

        public BusinessType BizzType
        {
            get => __type;
            set
            {
                __type = value;

                if(__type != null)
                    ShowIcon(__type.Icon);
                else
                    HideIcon();

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

            if(Owner != 0)
            {
                label += "Owner: " + Account.Account.GetSQLNameFromSQLID(Owner) + "\n\r";
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
                    cmd.Parameters.AddWithValue("@type", BizzType.Id);

                cmd.Parameters.AddWithValue("@domainid", Domainid);
                cmd.Parameters.AddWithValue("@id", __id);
                cmd.ExecuteNonQuery();
            }
        }

        public static Business FindByDomain(int domainid)
        {
            return GetAll<Business>().Where(b => b.Domainid == domainid).FirstOrDefault();
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
                Player newOnwer = Account.Account.GetPlayerBySQLID(ownerSqlID);

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
            Price = BizzType != null ? BizzType.Price : 0;
            UpdateLabel();
            UpdateSql();
        }
    }
}
