using System;

namespace Game
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        public static int GetRandomInt(int min = 0, int max = 100) => random.Next(min, max);

        public static (int dX, int dY) GetOffsetFromDirection(Direction direction) => direction switch
        {
            Direction.Left => (-1, 0),
            Direction.Right => (1, 0),
            Direction.Up => (0, -1),
            Direction.Down => (0, 1),
            _ => throw new ArgumentException(),
        };
    }
}
