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
            clientStream = tcpClient.GetStream();
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
                    Console.WriteLine("Received: " + data);

                    if (!authenticated)
                    {
                        // Authentication logic
                        if (data.StartsWith("LOGIN:"))
                        {
                            string[] parts = data.Substring(6).Split(':');
                            if (parts.Length == 2)
                            {
                                string username = parts[0];
                                string password = parts[1];

                                if (SQL_Handler.AuthenticateUser(username, password))
                                {
                                    authenticated = true;
                                    clientName = username;
                                    Console.WriteLine("User " + username + " authenticated");

                                    // Assign initial position
                                    (int startX, int startY) = server.GetNewPlayerPosition();
                                    x = startX;
                                    y = startY;
                                    server.AddNewPlayer(clientName);

                                    byte[] authResponse = Encoding.UTF8.GetBytes("Authentication successful\n");
                                    await clientStream.WriteAsync(authResponse, 0, authResponse.Length);
                                }
                                else
                                {
                                    byte[] authResponse = Encoding.UTF8.GetBytes("Authentication failed\n");
                                    await clientStream.WriteAsync(authResponse, 0, authResponse.Length);
                                }
                            }
                        }
                        else
                        {
                            byte[] response = Encoding.UTF8.GetBytes("Authentication required\n");
                            await clientStream.WriteAsync(response, 0, response.Length);
                        }
                    }
                    else
                    {
                        // Game logic
                        if (data.StartsWith("MOVE:"))
                        {
                            string[] parts = data.Substring(5).Split(':');
                            if (parts.Length == 2)
                            {
                                if (int.TryParse(parts[0], out int newX) && int.TryParse(parts[1], out int newY))
                                {
                                    if (Game.IsWithinBounds(newX, newY))
                                    {
                                        x = newX;
                                        y = newY;
                                        server.UpdatePlayerPosition(clientName, x, y);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                tcpClient.Close();
                clientHandlers.Remove(this); // Remove client from list when disconnected
                Console.WriteLine("Client disconnected");
            }
        }
    }
}
