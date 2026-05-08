using System.Collections.Generic;

namespace ASCIIAssault_Server
{
    public struct GameState
    {
        public Dictionary<string, (int x, int y)> PlayerPositions { get; set; } = new Dictionary<string, (int x, int y)>();
    }
}