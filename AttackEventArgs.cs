using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class AttackEventArgs:IEnumerable<Point>
    {
        public readonly Point[] Points;

        public AttackEventArgs(Point[] points) => Points = points;
        public AttackEventArgs(IEnumerable<Point> points) : this(points.ToArray()) { }

        public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)Points).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
    }
}