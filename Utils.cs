using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        public static int GetRandomInt(int min = 0, int max = 100) => random.Next(min, max);

        public static Direction GetDirectionFromOffset(Point offset) => GetDirectionFromOffset(offset.X, offset.Y);


        public static Direction GetDirectionFromOffset(int dX, int dY)
        => (dX, dY) switch
        {
            (-1, 0) => Direction.Left,
            (1, 0) => Direction.Right,
            (0, -1) => Direction.Up,
            (0, 1) => Direction.Down,
            _ => throw new ArgumentException(),
        };

        public static (int dX, int dY) GetOffsetFromDirection(Direction direction)
        => direction switch
        {
            Direction.Left => (-1, 0),
            Direction.Right => (1, 0),
            Direction.Up => (0, -1),
            Direction.Down => (0, 1),
            _ => throw new ArgumentException(),
        };

        public static double GetAngleFromDirection(Direction direction) => direction switch
        {
            Direction.Right => 0,
            Direction.Up => -Math.PI / 2,
            Direction.Left => Math.PI,
            Direction.Down => Math.PI / 2,
            _ => throw new NotImplementedException(),
        };

        public static IEnumerable<Point> RotatePoints(this IEnumerable<Point> points, double angle, Point origin = default)
        {
            var sine = Math.Sin(angle);
            var cosine = Math.Cos(angle);
            foreach (var point in points)
            {
                var x = (float)point.X - origin.X;
                var y = (float)point.Y - origin.Y;
                yield return Point.Truncate(new PointF((float)(x * cosine - y * sine + origin.X), (float)(x * sine + y * cosine + origin.Y)));
            }
        }

        public static IEnumerable<Point> RotatePoints(this IEnumerable<Point> points, Direction direction, Point origin = default) =>
            points.RotatePoints(GetAngleFromDirection(direction), origin);
    }
}
