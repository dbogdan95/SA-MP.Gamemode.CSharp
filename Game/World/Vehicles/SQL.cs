using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Vehicles
{
    public partial class Vehicle
    {
        public void Insert()
        {
            if (SQLID == null)
            {
                using (var conn = Database.Connect())
                {
                    var cmd = new MySqlCommand("INSERT INTO vehicles (model, x, y, z, a) VALUES (@model, @x, @y, @z, @a)", conn);
                    cmd.Parameters.AddWithValue("@model", Model);
                    cmd.Parameters.AddWithValue("@x", Position.X);
                    cmd.Parameters.AddWithValue("@y", Position.Y);
                    cmd.Parameters.AddWithValue("@z", Position.Z);
                    cmd.Parameters.AddWithValue("@a", Angle);
                    cmd.ExecuteNonQuery();

                    SQLID = (int)cmd.LastInsertedId;
                }
            }
        }

        public void Update(Vector3 pos, float angle, int color1, int color2)
        {
            using (var conn = Database.Connect())
            {
                var cmd = new MySqlCommand("UPDATE vehicles SET model=@model, x=@x, y=z@y, z=@z, a=@a, color1=@c1, color2=@c2", conn);
                cmd.Parameters.AddWithValue("@model", Model);
                cmd.Parameters.AddWithValue("@x", Position.X);
                cmd.Parameters.AddWithValue("@y", Position.Y);
                cmd.Parameters.AddWithValue("@z", Position.Z);
                cmd.Parameters.AddWithValue("@a", Angle);
                cmd.Parameters.AddWithValue("@c1", color1);
                cmd.Parameters.AddWithValue("@c2", color2);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
