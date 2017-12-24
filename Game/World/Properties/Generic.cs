using Game.Core;
using Game.Factions;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Properties
{
    public class Generic : Property
    {
        private int __id;

        private Generic(Interior interior, Vector3 pos, float angle, int id, int baseid)
            : base(baseid, PropertyType.TypeGeneric, interior, pos, angle)
        {
            __id = id;
        }

        public Generic(Interior interior, Vector3 pos, float angle) : base(PropertyType.TypeGeneric, interior, pos, angle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("INSERT INTO generic (baseProperty) VALUES (" + Id + ")", conn);
                cmd.ExecuteNonQuery();

                __id = (int)cmd.LastInsertedId;
            }
        }

        public Faction Faction { get; set; }

        public override void UpdateSql()
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("UPDATE generic SET baseFaction=@faction WHERE id=@id", conn);

                if(Faction == null)
                    cmd.Parameters.AddWithValue("@faction", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@faction", Faction.Id);

                cmd.Parameters.AddWithValue("@id", __id);
                cmd.ExecuteNonQuery();
            }
            base.UpdateSql();
        }

        public static void Load()
        {
            int props = 0;
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd = new MySqlCommand("SELECT A.id, A.baseFaction as faction, A.baseProperty, C.x, C.y, c.z, C.a, C.locked, C.deposit, C.interior " +
                    "FROM generic as A " +
                    "INNER JOIN properties AS C " +
                    "ON c.id = A.baseProperty", conn);

                MySqlDataReader data = cmd.ExecuteReader();

                while (data.Read())
                {
                    Generic g = new Generic(data["interior"] is DBNull ? null : Interior.FromIndex(data.GetInt32("interior")), new Vector3(data.GetFloat("x"), data.GetFloat("y"), data.GetFloat("z")), data.GetFloat("a"), data.GetInt32("id"), data.GetInt32("baseProperty"))
                    {
                        Deposit = data.GetInt32("deposit"),
                        Faction = data["faction"] is DBNull ? null : Faction.Find(data.GetInt32("faction"))
                    };
                    props++;
                }
                data.Close();
            }
            Console.WriteLine("** Loaded {0} generic properties from database.", props);
        }
        
        public override void SetOwnerUpdate(int id)
        {
            throw new NotImplementedException();
        }

        public override void UpdateLabel()
        {
            throw new NotImplementedException();
        }
    }
}
