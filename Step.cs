using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    public class Step
    {
        public Queue<Direction> Path = new Queue<Direction>(5);
        public Point Destination { get; private set; }

        public Step(Point initialPosition) => Destination = initialPosition;

        public void AddDirection(Direction direction)
        {
            Path.Enqueue(direction);
            var (dX, dY) = Utils.GetOffsetFromDirection(direction);
            Destination += new Size(dX, dY);
        }

        public IEnumerable<Direction> CommitStep()
        {
            while (Path.Count > 0)
                yield return Path.Dequeue();
        }

        public IEnumerable<Point> GetPreview(Point initialPosition)
        {
            yield return initialPosition;
            foreach (var direction in Path)
            {
                var (dX, dY) = Utils.GetOffsetFromDirection(direction);
                yield return initialPosition += new Size(dX, dY);
            }
        }
    }
}
