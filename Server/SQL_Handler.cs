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
            string? user = _config?["User"];
            string? password = _config?["Password"];

            // Construct the connection string
            string connectionString = $"Server={server};Port={port};Database={database};Uid={user};Pwd={password};";
            return connectionString;
        }

        public static bool CheckCredentials(string username, string password)
        {
            if (_config == null)
            {
                Console.WriteLine("Configuration is not initialized.");
                return false;
            }

            string connectionString = GetConnectionString();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {\r
                try
                {
                    connection.Open();

                    string query = "SELECT password_hash, password_salt FROM users WHERE username = @username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string passwordHashFromDb = reader.GetString("password_hash");
                                string passwordSaltFromDb = reader.GetString("password_salt");

                                // Verify the password
                                return PasswordHelper.VerifyPassword(password, passwordHashFromDb, passwordSaltFromDb);
                            }
                            else
                            {
                                // User not found
                                return false;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error connecting to the database: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
