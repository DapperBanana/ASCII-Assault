using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCIIAssault_Server
{
    public class Server
    {
        private TcpListener tcpListener;
        private List<ClientHandler> clientHandlers = new List<ClientHandler>();
        private GameState currentGameState = new GameState();
        private object gameStateLock = new object();
        private Random random = new Random();

        public void StartServer()
        {
            int port = 5000; // TODO: read from config
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            Console.WriteLine("Server started on port " + port);

            Task.Run(() =>
            {
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Client connected");
                    ClientHandler clientHandler = new ClientHandler(tcpClient, this);
                    clientHandlers.Add(clientHandler);
                    Task.Run(() => clientHandler.HandleClient());
                }
            });
        }

        public void BroadcastGameState()
        {
            lock (gameStateLock)
            {
                string gameStateString = GetGameStateString();
                foreach (var client in clientHandlers)
                {
                    // Removed: no broadcast to the client
                }
            }
        }

        private string GetGameStateString()
        {
            lock (gameStateLock)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var player in currentGameState.PlayerPositions)
                {
                    sb.AppendLine($"{player.Key}: ({player.Value.x}, {player.Value.y})");
                }
                return sb.ToString();
            }
        }

        public void UpdatePlayerPosition(string playerName, int x, int y)
        {
            lock (gameStateLock)
            {
                if (currentGameState.PlayerPositions.ContainsKey(playerName))
                {
                    currentGameState.PlayerPositions[playerName] = (x, y);
                }
            }
            BroadcastGameState();
        }

        public (int x, int y) GetNewPlayerPosition()
        {
            int x, y;
            do
            {
                x = random.Next(0, 20); // Assuming Game.MaxX = 20
                y = random.Next(0, 20); // Assuming Game.MaxY = 20
            } while (currentGameState.PlayerPositions.Values.Any(pos => pos.x == x && pos.y == y));
            return (x, y);
        }

        public void AddNewPlayer(string playerName)
        {
            lock (gameStateLock)
            {
                (int x, int y) newPos = GetNewPlayerPosition();
                currentGameState.PlayerPositions.Add(playerName, newPos);
            }
        }
    }
}
