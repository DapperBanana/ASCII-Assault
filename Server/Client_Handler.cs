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

        public void ProcessClient()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + dataReceived);

                    if (!authenticated)
                    {
                        HandleAuthentication(dataReceived);
                    }
                    else
                    {
                        HandleCommand(dataReceived);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            finally
            {
                lock (server.clientsLock)
                {
                    server.clients.Remove(this);
                }
                tcpClient.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        private void HandleAuthentication(string data)
        {
            if (data.StartsWith("AUTH:"))
            {
                string[] parts = data.Substring(5).Split(':');
                if (parts.Length == 2)
                {
                    string username = parts[0];
                    string password = parts[1];

                    string hashedPassword = SQL_Handler.GetHashedPassword(username);

                    if (hashedPassword != null && PasswordHelper.VerifyPassword(password, hashedPassword))
                    {
                        authenticated = true;
                        clientName = username;
                        SendMessage("AUTH_OK");
                        Console.WriteLine("Client authenticated: " + username);
                    }
                    else
                    {
                        SendMessage("AUTH_FAIL");
                        Console.WriteLine("Authentication failed for: " + username);
                        tcpClient.Close(); // Close connection on failed auth
                    }
                }
                else
                {
                    SendMessage("AUTH_INVALID");
                    tcpClient.Close(); // Close connection on invalid auth format
                }
            }
            else
            {
                SendMessage("AUTH_REQUIRED");
                tcpClient.Close(); // Close connection if no auth is provided
            }
        }

        private void HandleCommand(string data)
        {\r
            if (data.StartsWith("MOVE:"))
            {
                string[] parts = data.Substring(5).Split(':');
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int dx) && int.TryParse(parts[1], out int dy))
                    {
                        int newX = x + dx;
                        int newY = y + dy;

                        if (Game.IsWithinBounds(newX, newY))
                        {
                            x = newX;
                            y = newY;
                            Console.WriteLine($"Client {clientName} moved to X:{x}, Y:{y}");
                            server.BroadcastMessage($"UPDATE:{clientName}:{x}:{y}", this);
                        }
                        else
                        {
                            SendMessage("Invalid move, out of bounds");
                        }
                    }
                    else
                    {
                        SendMessage("Invalid move format");
                    }
                }
                else
                {
                    SendMessage("Invalid move format");
                }
            }
            else
            {
                SendMessage("Unknown command");
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(message);
                clientStream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }
    }
}
