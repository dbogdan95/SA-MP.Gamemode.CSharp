using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;

namespace Game.Core
{
    public class Database
    {
        static string __ConStr { get; set; }

        static Database()
        {
            Console.WriteLine("Database");
            var builder = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("config.json");

            IConfigurationRoot Config = builder.Build();

            var authentication = "";

            if (Convert.ToBoolean(Config["database_using_ssl"]))
            {
                authentication = ";CertificateFile=" + Config["database_ssl_pfx_file"] +
                                 ";CertificatePassword=" + Config["database_ssl_pfx_password"];
            }
            else
            {
                authentication = ";password=" + Config["database_password"] + ";SslMode=none";
            }
            __ConStr = "server=" + Config["database_server"] +
                      ";user=" + Config["database_user"] +
                      ";database=" + Config["database_database"] +
                      ";port=" + Config["database_port"] +
                      authentication +
                      ";min pool size=" + Config["database_min_pool_size"] +
                      ";max pool size=" + Config["database_max_pool_size"] + ";";
        }

        public static MySqlConnection Connect()
        {
            var conn = new MySqlConnection(__ConStr);
            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                return null;
            }
        }

        /*public static void Connect()
        {
            using (MySqlConnection conn = new MySqlConnection(__ConStr))
            {
                try
                {
                    Console.WriteLine("DATABASE: [INFO] Attempting connecting to MySQL");
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        Console.WriteLine("DATABASE: [INFO] Connected to MySQL");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                }
            }
        }*/

        /* Exports */
        //public static MySqlDataReader ExecuteQueryWithResult(string sql)
        //{
        //    try
        //    {
        //        MySqlConnection conn = new MySqlConnection(__ConStr);
        //        conn.Open();

        //        MySqlCommand cmd = new MySqlCommand(sql, conn);
        //        MySqlDataReader r = cmd.ExecuteReader();
        //        return r;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //        return null;
        //    }
        //}

        //public static int ExecuteNonQuery(string commandText)
        //{
        //    int affectedRows = 0;
        //    using (var connection = Connect())
        //    {
        //        using (var command = new MySqlCommand(commandText, connection))
        //        {
        //            affectedRows = command.ExecuteNonQuery();
        //        }
        //    }
        //    return affectedRows;
        //}

        //public static DataTable ExecutePreparedQueryWithResult(string sql, Dictionary<string, string> parameters)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(__ConStr))
        //    {
        //        try
        //        {
        //            MySqlCommand cmd = new MySqlCommand(sql, conn);
        //            conn.Open();


        //            foreach (KeyValuePair<string, string> entry in parameters)
        //            {
        //                cmd.Parameters.AddWithValue(entry.Key, entry.Value);
        //            }

        //            MySqlDataReader rdr = cmd.ExecuteReader();
        //            DataTable results = new DataTable();
        //            results.Load(rdr);
        //            rdr.Close();
        //            return results;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //            return null;
        //        }
        //    }
        //}

        //public static MySqlCommand ExecuteQuery(string sql)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(__ConStr))
        //    {
        //        try
        //        {
        //            MySqlCommand cmd = new MySqlCommand(sql, conn);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            return cmd;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //            return null;
        //        }
        //    }
        //}

        //public static void ExecutePreparedQuery(string sql, Dictionary<string, string> parameters)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(__ConStr))
        //    {
        //        try
        //        {
        //            MySqlCommand cmd = new MySqlCommand(sql, conn);
        //            conn.Open();
        //            foreach (KeyValuePair<string, string> entry in parameters)
        //            {
        //                cmd.Parameters.AddWithValue(entry.Key, entry.Value);
        //            }
        //            cmd.ExecuteNonQuery();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //        }
        //    }
        //}

        //public DataTable CreateDataTable(string sql, string unique_name)
        //{
        //    using (MySqlConnection conn = new MySqlConnection(connStr))
        //    {
        //        try
        //        {
        //            MySqlDataAdapter dataAdapter;
        //            DataTable dataTable;

        //            dataAdapter = new MySqlDataAdapter(sql, conn);
        //            MySqlCommandBuilder cb = new MySqlCommandBuilder(dataAdapter);
        //            dataAdapters[unique_name] = dataAdapter;
        //            dataTable = new DataTable();
        //            dataAdapter.Fill(dataTable);
        //            return dataTable;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //            return null;
        //        }
        //    }
        //}

        //public void UpdateDataTable(string unique_name, DataTable updatedTable)
        //{
        //    try
        //    {
        //        dataAdapters[unique_name].Update(updatedTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //    }
        //}

        //public void CloseDataTable(string unique_name)
        //{
        //    try
        //    {
        //        MySqlDataAdapter data = dataAdapters[unique_name];
        //        dataAdapters.Remove(unique_name);
        //        data.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
        //    }
        //}
    }
}
