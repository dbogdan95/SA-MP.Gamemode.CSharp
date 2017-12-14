using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System.Linq;

namespace Game.World.Property.Business
{
    public class Business : Property
    {
        private long __SQLID;
        private BusinessType __type;
        private int __domainid;

        public Business(Interior interior, Vector3 pos, float angle) 
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO business (interior, x, y, z, a, locked) VALUES (@interior, @x, @y, @z, @a, @locked)", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.Index(interior));
                cmd.Parameters.AddWithValue("@x", pos.X);
                cmd.Parameters.AddWithValue("@y", pos.Y);
                cmd.Parameters.AddWithValue("@z", pos.Z);
                cmd.Parameters.AddWithValue("@a", angle);
                cmd.Parameters.AddWithValue("@locked", Locked);

                // Those are default values in MySql
                //cmd.Parameters.AddWithValue("@type", (int)__type);
                //cmd.Parameters.AddWithValue("@domainid", __domainid);
                cmd.ExecuteNonQuery();

                __SQLID = cmd.LastInsertedId;
            }
            BizzType = null;
        }
        public Business(Interior interior, Vector3 pos, float angle, int sqlid)
            : base(PropertyType.TypeBusiness, interior, pos, angle)
        {
            __SQLID = sqlid;
            BizzType = null;
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

            if (__type != null)
                label += "[Business - " + __type.ToString() + "]\n\r";
            else
                label += "[Business]\n\r";

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
                MySqlCommand cmd = new MySqlCommand("UPDATE business SET interior=@interior, locked=@locked, deposit=@deposit, type=@type, domainid=@domainid WHERE id=@id", conn);
                cmd.Parameters.AddWithValue("@interior", Interior.Index(Interior));
                cmd.Parameters.AddWithValue("@locked", Locked);
                cmd.Parameters.AddWithValue("@deposit", Deposit);
                cmd.Parameters.AddWithValue("@type", BizzType?.Id);
                cmd.Parameters.AddWithValue("@domainid", Domainid);
                cmd.Parameters.AddWithValue("@id", __SQLID);
                cmd.ExecuteNonQuery();
            }
        }
        public static Business GetBusinessByDomainID(int domainid)
        {
            return GetAll<Business>().Where(b => b.Domainid == domainid).FirstOrDefault();
        }
        public override int GetSqlID()
        {
            return (int)__SQLID;
        }

        public override void SetOwnerUpdate(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
