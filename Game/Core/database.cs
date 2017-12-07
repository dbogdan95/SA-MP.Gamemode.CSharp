using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Game.Core
{
    public class Database
    {
        private static string connStr;
        public static IConfigurationRoot Configuration { get; set; }

        public static void Connect()
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json");

            Configuration = builder.Build();

            var authentication = "";

            if (Convert.ToBoolean(Configuration["database_using_ssl"]))
            {
                authentication = ";CertificateFile=" + Configuration["database_ssl_pfx_file"] +
                                 ";CertificatePassword=" + Configuration["database_ssl_pfx_password"];
            }
            else
            {
                authentication = ";password=" + Configuration["database_password"]+ ";SslMode=none";
            }
            connStr = "server=" + Configuration["database_server"] +
                      ";user=" + Configuration["database_user"] +
                      ";database=" + Configuration["database_database"] +
                      ";port=" + Configuration["database_port"] +
                      authentication +
                      ";min pool size=" + Configuration["database_min_pool_size"] +
                      ";max pool size=" + Configuration["database_max_pool_size"] + ";";

            using (MySqlConnection conn = new MySqlConnection(connStr))
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
        }

        /* Exports */
        public static DataTable ExecuteQueryWithResult(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    DataTable results = new DataTable();
                    results.Load(rdr);
                    rdr.Close();
                    return results;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                    return null;
                }
            }
        }

        public static DataTable ExecutePreparedQueryWithResult(string sql, Dictionary<string, string> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();


                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    DataTable results = new DataTable();
                    results.Load(rdr);
                    rdr.Close();
                    return results;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                    return null;
                }
            }
        }

        public static void ExecuteQuery(string sql)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                }
            }
        }

        public static void ExecutePreparedQuery(string sql, Dictionary<string, string> parameters)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    foreach (KeyValuePair<string, string> entry in parameters)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DATABASE: [ERROR] " + ex.ToString());
                }
            }
        }

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
