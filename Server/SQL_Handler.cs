using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ASCIIAssault_Server
{
    public class SQL_Handler
    {
        private static IConfiguration? _config;

        public static void SetConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public static string GetConnectionString()
        {
            string? server = _config?["Server"];
            string? port = _config?["Port"];
            string? database = _config?["Database"];
            string? uid = _config?["Uid"];
            string? password = _config?["Password"];

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Missing required configuration settings for database connection.");
            }

            return $"Server={server};Port={port};Database={database};Uid={uid};Pwd={password};";
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }
    }
}
