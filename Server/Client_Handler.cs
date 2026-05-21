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
            clientStream = client.GetStream();
        }

        public async Task HandleClient()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from client: {data}");

                    // Handle authentication first
                    if (!authenticated)
                    {
                        if (data.StartsWith("LOGIN:"))
                        {
                            string[] credentials = data.Substring(6).Split(':');
                            if (credentials.Length == 2)
                            {
                                string username = credentials[0];
                                string password = credentials[1];

                                if (SQL_Handler.AuthenticateUser(username, password))
                                {
                                    clientName = username;
                                    authenticated = true;

                                    // Set initial position if it doesn't exist
                                    if (!server.gameState.PlayerPositions.ContainsKey(clientName))
                                    {
                                        var initialPosition = server.GetAvailableSpawnPoint();
                                        x = initialPosition.x;
                                        y = initialPosition.y;
                                        server.gameState.PlayerPositions[clientName] = (x, y);
                                    }

                                    string response = "LOGIN_SUCCESS";
                                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                    await clientStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                                    server.BroadcastGameState();
                                }
                                else
                                {
                                    string response = "LOGIN_FAILED";
                                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                                    await clientStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                                }
                            }
                        }
                        else
                        {
                            string response = "INVALID_COMMAND";
                            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                            await clientStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        }
                    }
                    else
                    {
                        // Handle game commands
                        if (data.Equals("MOVE_UP") && Game.IsWithinBounds(x, y - 1))
                        {
                            y--;
                            server.gameState.PlayerPositions[clientName] = (x, y);
                            server.BroadcastGameState();
                        }
                        else if (data.Equals("MOVE_DOWN") && Game.IsWithinBounds(x, y + 1))
                        {
                            y++;
                            server.gameState.PlayerPositions[clientName] = (x, y);
                            server.BroadcastGameState();
                        }
                        else if (data.Equals("MOVE_LEFT") && Game.IsWithinBounds(x - 1, y))
                        {
                            x--;
                            server.gameState.PlayerPositions[clientName] = (x, y);
                            server.BroadcastGameState();
                        }
                        else if (data.Equals("MOVE_RIGHT") && Game.IsWithinBounds(x + 1, y))
                        {
                            x++;
                            server.gameState.PlayerPositions[clientName] = (x, y);
                            server.BroadcastGameState();
                        }
                        else
                        {
                            string response = "INVALID_COMMAND";
                            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                            await clientStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                Console.WriteLine("Client disconnected.");
                server.RemoveClient(this);
                tcpClient.Close();
            }
        }
    }
}
