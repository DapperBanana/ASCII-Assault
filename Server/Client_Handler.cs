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

        public string? ClientName { get => clientName; set => clientName = value; }
        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public bool IsAuthenticated()
        {
            return authenticated;
        }

        public void RunClient()
        {
            try
            {
                HandleClientCommunication();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                server.RemoveClient(this);
                tcpClient.Close();
                Console.WriteLine("Client disconnected");
            }
        }

        private void HandleClientCommunication()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = clientStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received: " + data);

                string[] commands = data.Split(';');

                foreach (string command in commands)
                {
                    string trimmedCommand = command.Trim();
                    if (string.IsNullOrEmpty(trimmedCommand))
                    {
                        continue;
                    }

                    string[] parts = trimmedCommand.Split(' ');
                    string action = parts[0];

                    switch (action.ToLower())
                    {
                        case "login":
                            if (parts.Length == 3)
                            {
                                string username = parts[1];
                                string password = parts[2];
                                if (SQL_Handler.VerifyPassword(username, password))
                                {
                                    authenticated = true;
                                    ClientName = username;
                                    SendMessage("Login successful");

                                    //Initial game state broadcast to new client
                                    GameState initialGameState = server.GetGameState();
                                    string gameStateString = ConvertGameStateToString(initialGameState);
                                    SendMessage(gameStateString);
                                }
                                else
                                {
                                    SendMessage("Login failed");
                                }
                            }
                            break;

                        case "move":
                            if (authenticated)
                            {
                                if (parts.Length == 3)
                                {
                                    if (int.TryParse(parts[1], out int newX) && int.TryParse(parts[2], out int newY))
                                    {
                                        if (Game.IsWithinBounds(newX, newY))
                                        {
                                            X = newX;
                                            Y = newY;
                                            server.Broadcast("Player " + ClientName + " moved to " + X + "," + Y, this);
                                            //Broadcast current game state after move
                                            GameState currentGameState = server.GetGameState();
                                            string gameStateString = ConvertGameStateToString(currentGameState);
                                            server.Broadcast(gameStateString, this);
                                        }
                                        else
                                        {
                                            SendMessage("Invalid move: Out of bounds");
                                        }
                                    }
                                    else
                                    {
                                        SendMessage("Invalid move: Coordinates must be integers");
                                    }
                                }
                            }
                            else
                            {
                                SendMessage("Authentication required");
                            }
                            break;

                        default:
                            SendMessage("Invalid command");
                            break;
                    }
                }
            }
        }

        private string ConvertGameStateToString(GameState gameState)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GAMEOBJ");

            foreach (var playerPosition in gameState.PlayerPositions)
            {
                sb.AppendLine($"Player: {playerPosition.Key}, X: {playerPosition.Value.x}, Y: {playerPosition.Value.y}");
            }
            sb.AppendLine("ENDOBJ");

            return sb.ToString();
        }


        public void SendMessage(string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
            clientStream.Write(data, 0, data.Length);
        }
    }
}