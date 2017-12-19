using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using System.Linq;
using System;
using Game.World.Players;
using Game.Accounts;

namespace Game.World.Properties
{
    public class Business : Property
    {
        private int __id;
        private BusinessType __type;
        private int __domainid;
        
        static Business()
        {
            new BusinessType(BusinessTypes.TypeDealership, 55, "DealerShip", 100000);
            new BusinessType(BusinessTypes.TypeAmmo, 6, "Ammunition", 50000);
            new BusinessType(BusinessTypes.TypePaynSpray, 63, "Pay'n'Spray", 40000);
            new BusinessType(BusinessTypes.TypeModShop, 27, "ModShop", 30000);
            new BusinessType(BusinessTypes.TypeBurger, 10, "Burget Shot", 60000);
            new BusinessType(BusinessTypes.TypeCluckin, 14, "Cluckin Bell", 60000);
            new BusinessType(BusinessTypes.TypePizza, 29, "Pizza Stack", 60000);
            new BusinessType(BusinessTypes.TypeHospital, 22, "Hospital", 500000);
            new BusinessType(BusinessTypes.TypeClub, 48, "Night Club", 50000);
            new BusinessType(BusinessTypes.TypeSex, 21, "Sex Shop", 30000);
            new BusinessType(BusinessTypes.TypeBarber, 7, "Barber", 20000);
            new BusinessType(BusinessTypes.TypeBank, 52, "Bank", 40000);
            new BusinessType(BusinessTypes.TypeGas, -1, "Gas Station", 10000);
        }

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
            : base(sqlid, PropertyType.TypeBusiness, interior, pos, angle)
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
                    cmd.Parameters.AddWithValue("@type", BizzType.Id);

                cmd.Parameters.AddWithValue("@domainid", Domainid);
                cmd.Parameters.AddWithValue("@id", __id);
                cmd.ExecuteNonQuery();
            }
            base.UpdateSql();
        }

        public static Business FindByDomain(int domainid, BusinessTypes type)
        {
            return GetAll<Business>().Where(b => b.BizzType == BusinessType.Find((int)type) && b.Domainid == domainid)
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
            Price = BizzType != null ? BizzType.Price : 0;
            UpdateLabel();
            UpdateSql();
        }
    }
}
