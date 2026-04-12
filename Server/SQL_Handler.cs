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

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(port) || string.IsNullOrEmpty(database) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Missing database configuration in appsettings.json");
                return string.Empty;
            }

            return $"Server={server};Port={port};Database={database};Uid={user};Pwd={password};";
        }

        public static bool AuthenticateUser(string username, string password)
        {
            string connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT password FROM users WHERE username = @username";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        object? result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            string hashedPassword = result.ToString();
                            return PasswordHelper.VerifyPassword(password, hashedPassword);
                        }
                        else
                        {
                            Console.WriteLine($"User {username} not found.");
                            return false;
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine($"Database error: {ex.Message}");
                    return false;
                }
            }
        }
    }
}