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

                // Send initial positions to new client
                SendInitialPositions();

                while ((bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string[] parts = data.Split(' ');

                    if (parts.Length > 0)
                    {
                        string command = parts[0].ToLower();

                        switch (command)
                        {
                            case "login":
                                if (parts.Length == 3)
                                {
                                    string username = parts[1];
                                    string password = parts[2];
                                    if (Authenticate(username, password))
                                    {
                                        clientName = username;
                                        authenticated = true;

                                        // Send confirmation message
                                        string authMessage = "Authentication successful.\n";
                                        byte[] authMessageBytes = Encoding.ASCII.GetBytes(authMessage);
                                        await clientStream.WriteAsync(authMessageBytes, 0, authMessageBytes.Length);

                                        // Get initial position from server
                                        (x, y) = server.GetPlayerPosition(clientName);
                                        Console.WriteLine($"Client {clientName} connected from {tcpClient.Client.RemoteEndPoint} at position ({x}, {y})");
                                    }
                                    else
                                    {
                                        // Send authentication failure message
                                        string failMessage = "Authentication failed.\n";
                                        byte[] failMessageBytes = Encoding.ASCII.GetBytes(failMessage);
                                        await clientStream.WriteAsync(failMessageBytes, 0, failMessageBytes.Length);
                                    }
                                }
                                break;

                            case "move":
                                if (authenticated && parts.Length == 2)
                                {\n                                    string direction = parts[1];

                                    // Calculate new position using Game class
                                    (int newX, int newY) = Game.CalculateNewPosition(x, y, direction);

                                    // Check bounds using Game class
                                    if (Game.IsWithinBounds(newX, newY))
                                    {
                                        x = newX;
                                        y = newY;

                                        // Update server's game state
                                        server.UpdatePlayerPosition(clientName, (x, y));
                                    }
                                    else
                                    {
                                        // Send out of bounds message
                                        string oobMessage = "Cannot move in that direction, out of bounds.\n";
                                        byte[] oobMessageBytes = Encoding.ASCII.GetBytes(oobMessage);
                                        await clientStream.WriteAsync(oobMessageBytes, 0, oobMessageBytes.Length);
                                    }

                                    // Broadcast updated positions to all clients
                                    server.BroadcastPlayerPositions();
                                }
                                break;

                            default:
                                Console.WriteLine($"Received: {data} from Client {tcpClient.Client.RemoteEndPoint}");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ClientHandler: {ex.Message}");
            }
            finally
            {
                Console.WriteLine($"Client disconnected: {tcpClient.Client.RemoteEndPoint}");
                server.RemoveClient(clientName);
                tcpClient.Close();
            }
        }

        private bool Authenticate(string username, string password)
        {
            return SQL_Handler.VerifyPassword(username, password);
        }

        // Send initial positions of all players to the new client
        private async void SendInitialPositions()
        {
            if (clientStream != null)
            {
                string positions = server.GetCurrentPlayerPositions();
                byte[] positionsBytes = Encoding.ASCII.GetBytes(positions);
                await clientStream.WriteAsync(positionsBytes, 0, positionsBytes.Length);
            }
        }
    }
}
