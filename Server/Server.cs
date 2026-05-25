using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class Server
    {
        private TcpListener tcpListener;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private GameState gameState = new GameState();
        private object gameStateLock = new object();
        private int nextAvailableX = 1;
        private int nextAvailableY = 1;
        private object positionLock = new object();

        public Server()
        {
            // Initialize the TCP listener on all available IPs and port 5000
            tcpListener = new TcpListener(IPAddress.Any, 5000);
        }

        public void StartServer()
        {
            tcpListener.Start();
            Console.WriteLine("Server started. Listening for connections...");

            // Start listening for client connections asynchronously
            AcceptClientsAsync();
        }

        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                Console.WriteLine($"Client connected from {tcpClient.Client.RemoteEndPoint}");

                ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                clients.Add(clientHandler);

                // Handle each client in a separate task
                _ = Task.Run(() => clientHandler.HandleClient());
            }
        }

        public (int x, int y) GetPlayerPosition(string clientName)
        {
            lock (gameStateLock)
            {
                if (!gameState.PlayerPositions.ContainsKey(clientName))
                {\n                    //Assign initial position
                    return AssignInitialPosition(clientName);
                }
                return gameState.PlayerPositions[clientName];
            }
        }

        private (int x, int y) AssignInitialPosition(string clientName)
        {
            lock (positionLock)
            {
                int x = nextAvailableX;
                int y = nextAvailableY;

                nextAvailableX++;
                if (nextAvailableX > 18)
                {
                    nextAvailableX = 1;
                    nextAvailableY++;
                    if (nextAvailableY > 18)
                    {
                        nextAvailableY = 1;
                    }
                }

                lock (gameStateLock)
                {
                    gameState.PlayerPositions[clientName] = (x, y);
                }

                return (x, y);
            }
        }

        public void UpdatePlayerPosition(string clientName, (int x, int y) newPosition)
        {
            lock (gameStateLock)
            {
                gameState.PlayerPositions[clientName] = newPosition;
            }
        }

        public void RemoveClient(string clientName)
        {
            lock (gameStateLock)
            {
                gameState.PlayerPositions.Remove(clientName);
            }
        }

        public async void BroadcastPlayerPositions()
        {
            string positions = GetCurrentPlayerPositions();
            byte[] data = Encoding.ASCII.GetBytes(positions);

            foreach (var client in clients)
            {
                try
                {
                    NetworkStream stream = client.tcpClient.GetStream();
                    await stream.WriteAsync(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error broadcasting to client: {ex.Message}");
                }
            }
        }

        public string GetCurrentPlayerPositions()
        {
            StringBuilder sb = new StringBuilder();
            lock (gameStateLock)
            {
                foreach (var player in gameState.PlayerPositions)
                {
                    sb.AppendLine($"POS {player.Key} {player.Value.x} {player.Value.y}");
                }
            }

            return sb.ToString();
        }
    }
}
