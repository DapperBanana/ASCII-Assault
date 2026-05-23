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
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received from client: {data}");

                    string[] parts = data.Split(' ');

                    if (parts.Length > 0)
                    {
                        string command = parts[0].ToLower();

                        switch (command)
                        {
                            case "move":
                                if (authenticated && parts.Length == 2)
                                {
                                    string direction = parts[1].ToLower();
                                    (int newX, int newY) = Game.CalculateNewPosition(x, y, direction);

                                    if (Game.IsWithinBounds(newX, newY))
                                    {
                                        x = newX;
                                        y = newY;
                                        server.UpdatePlayerPosition(clientName, x, y);
                                        Console.WriteLine($"Client {clientName} moved {direction} to ({x}, {y})");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Client {clientName} tried to move out of bounds.");
                                    }
                                }
                                break;

                            case "authenticate":
                                if (parts.Length == 3)
                                {
                                    string username = parts[1];
                                    string password = parts[2];

                                    if (SQL_Handler.AuthenticateUser(username, password))
                                    {
                                        clientName = username;
                                        authenticated = true;
                                        Console.WriteLine($"Client {clientName} authenticated successfully.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Authentication failed for {username}.");
                                    }
                                }
                                break;

                            default:
                                Console.WriteLine($"Invalid command from client: {command}");
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error handling client: {e.Message}");
            }
            finally
            {
                Console.WriteLine("Client disconnected.");
                server.RemoveClient(clientName);
                tcpClient.Close();
            }
        }
    }
}