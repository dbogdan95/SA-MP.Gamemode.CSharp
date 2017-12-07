using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Game
{
    class SQL
    {
        MySqlConnection conn = null;

        public SQL(string host, string user, string password, string database)
        {
            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = "server="+ host + ";uid="+ user + ";pwd="+ password + ";database="+ database;
                conn.Open();
                conn.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
