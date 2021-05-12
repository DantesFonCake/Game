using Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public static class Utils
    {
        private static readonly Random random = new Random();

        public static int GetRandomInt(int min = 0, int max = 100) => random.Next(min, max);

        public static Bitmap SetOpacity(this Bitmap image, int opacity, bool changeTransparent = false)
        {
            var im = new Bitmap(image);
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    var originalColor = im.GetPixel(x, y);
                    if (!changeTransparent && originalColor.A == 0)
                        continue;
                    im.SetPixel(x, y, Color.FromArgb(opacity, originalColor));
                }
            }
            return im;
        }

        public static Direction Copy(this Direction en) => (Direction)(int)en;

        public static Direction GetDirectionFromOffset(this Point offset) => GetDirectionFromOffset(offset.X, offset.Y);
        public static Direction KeyCodeToDirection(this Keys keyCode)
            => keyCode switch
            {
                Keys.W => Direction.Up,
                Keys.Up => Direction.Up,
                Keys.D => Direction.Right,
                Keys.Right => Direction.Right,
                Keys.A => Direction.Left,
                Keys.Left => Direction.Left,
                Keys.S => Direction.Down,
                Keys.Down => Direction.Down,
                _ => Direction.None
            };

        public static IEnumerable<Point> GetAdjancent(this Point point)
        {
            var offsets = new[] { new Size(-1, 0), new Size(1, 0), new Size(0, 1), new Size(0, -1) };
            return offsets.Select(x => point + x);
        }

        public static double GetDistance(this Point point, Point point2) => 
            Math.Sqrt((point.X - point2.X) * (point.X - point2.X) +
                (point.Y - point2.Y) * (point.Y - point2.Y));

        public static IEnumerable<Size> GetRadius(int radius)
        {
            var sizes = Enumerable.Range(0, radius).Select(x => new Size(x, 0));
            return Enumerable.Range(0, radius * 8 + 1).SelectMany(x => sizes.RotateSizes(x * 2 * Math.PI / (radius * 8))).Distinct();
        }

        public static Direction GetDirectionFromOffset(int dX, int dY)
        => (dX, dY) switch
        {
            (-1, 0) => Direction.Left,
            (1, 0) => Direction.Right,
            (0, -1) => Direction.Up,
            (0, 1) => Direction.Down,
            _ => Direction.None,
        };

        public static Size GetOffsetFromDirection(this Direction direction)
        => direction switch
        {
            Direction.Left => new Size(-1, 0),
            Direction.Right => new Size(1, 0),
            Direction.Up => new Size(0, -1),
            Direction.Down => new Size(0, 1),
            _ => throw new ArgumentException(),
        };

        public static double GetAngleFromDirection(this Direction direction) => direction switch
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
                var x = (double)point.X - origin.X;
                var y = (double)point.Y - origin.Y;
                yield return new Point((int)Math.Round(x * cosine - y * sine)+origin.X, (int)Math.Round(x * sine + y * cosine)+origin.Y);
            }
        }

        public static IEnumerable<Size> RotateSizes(this IEnumerable<Size> sizes, double angle)
        {
            var sine = Math.Sin(angle);
            var cosine = Math.Cos(angle);
            foreach (var size in sizes)
            {
                var x = (double)size.Width;
                var y = (double)size.Height;
                yield return new Size((int)Math.Ceiling(x * cosine - y * sine), (int)Math.Ceiling(x * sine + y * cosine));
            }
        }

        public static IEnumerable<Point> RotatePoints(this IEnumerable<Point> points, Direction direction, Point origin = default) =>
            points.RotatePoints(GetAngleFromDirection(direction), origin);

        public static Dictionary<TValue, TKey> ToFlippedDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in source)
            {
                if (!dictionary.ContainsKey(entry.Value))
                    dictionary.Add(entry.Value, entry.Key);
            }
            return dictionary;
        }
    }
}
