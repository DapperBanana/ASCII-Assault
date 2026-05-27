using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class ClientHandler
    {
        private TcpClient tcpClient;
        private Server server;
        private NetworkStream clientStream;
        private string? clientName;
        private bool authenticated = false;
        private int x = 0; // Client's X position
        private int y = 0; // Client's Y position


        public ClientHandler(TcpClient client, Server server)
        {
            this.tcpClient = client;
            this.server = server;
        }

        public void RunClient()
        {
            clientStream = tcpClient.GetStream();
            HandleClient();
        }

        private void HandleClient()
        {
            try
            {
                byte[] message = new byte[4096];
                int bytesRead;

                // Handle authentication first
                if (!AuthenticateClient()) return;


                // Send initial game state to the client
                SendInitialGameState();

                while (true)
                {
                    bytesRead = 0;
                    try
                    {
                        bytesRead = clientStream.Read(message, 0, message.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading from client: " + ex.Message);
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        // Client disconnected
                        Console.WriteLine("Client disconnected");
                        break;
                    }

                    string command = Encoding.UTF8.GetString(message, 0, bytesRead);
                    Console.WriteLine("Received command: " + command);
                    ProcessCommand(command);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private bool AuthenticateClient()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (request.StartsWith("AUTH "))
            {
                string[] parts = request.Substring(5).Split(':');
                if (parts.Length == 2)
                {
                    string username = parts[0];
                    string password = parts[1];

                    if (SQL_Handler.VerifyPassword(username, password))
                    {
                        clientName = username;
                        authenticated = true;
                        Console.WriteLine($"Client {username} authenticated successfully.");
                        byte[] response = Encoding.UTF8.GetBytes("AUTH_OK");
                        clientStream.Write(response, 0, response.Length);
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Authentication failed for {username}.");
                        byte[] response = Encoding.UTF8.GetBytes("AUTH_FAIL");
                        clientStream.Write(response, 0, response.Length);
                        return false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Authentication attempt failed.");
                byte[] response = Encoding.UTF8.GetBytes("AUTH_REQUIRED");
                clientStream.Write(response, 0, response.Length);
                return false;
            }

            return false;
        }

        private void ProcessCommand(string command)
        {
            command = command.Trim().ToLower();

            string[] parts = command.Split(' ');

            if (parts.Length > 0)
            {
                switch (parts[0])
                {\n                    case "move":
                        if (parts.Length == 2)
                        {
                            string direction = parts[1];
                            (int newX, int newY) = Game.CalculateNewPosition(x, y, direction);

                            if (Game.IsWithinBounds(newX, newY))
                            {
                                x = newX;
                                y = newY;

                                Console.WriteLine($"Client {clientName} moved {direction} to ({x}, {y})");
                                //TODO: update game state and broadcast to all clients


                            }
                            else
                            {
                                Console.WriteLine($"Client {clientName} attempted to move out of bounds.");
                            }
                        }
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {command}");
                        break;
                }
            }
        }

        private void SendInitialGameState()
        {
            // Get the current player positions
            GameState gameState = Game.GetCurrentPlayerPositions();

            // Serialize the game state to JSON
            string gameStateJson = System.Text.Json.JsonSerializer.Serialize(gameState);

            // Send the game state to the client
            byte[] gameStateBytes = Encoding.UTF8.GetBytes(gameStateJson);
            clientStream.Write(gameStateBytes, 0, gameStateBytes.Length);

            Console.WriteLine("Sent initial game state to client.");
        }
    }
}
