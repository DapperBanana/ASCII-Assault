namespace ASCIIAssault_Server
{
    public static class Game
    {
        private const int MaxX = 20; // Maximum X coordinate
        private const int MaxY = 20; // Maximum Y coordinate

        public static bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < MaxX && y >= 0 && y < MaxY;
        }
    }
}