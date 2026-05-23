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

        public static (int, int) CalculateNewPosition(int currentX, int currentY, string direction)
        {
            switch (direction.ToLower())
            {
                case "north":
                    return (currentX, currentY - 1);
                case "south":
                    return (currentX, currentY + 1);
                case "east":
                    return (currentX + 1, currentY);
                case "west":
                    return (currentX - 1, currentY);
                default:
                    return (currentX, currentY); // Invalid direction, no movement
            }
        }
    }
}