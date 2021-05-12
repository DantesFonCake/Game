using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class AttackEventArgs : IEnumerable<Point>
    {
        public readonly Point[] Points;
        public readonly Attack Attack;

        public AttackEventArgs(Point[] points, Attack attack)
        {
            Points = points;
            Attack = attack;
        }

        public AttackEventArgs(IEnumerable<Point> points, Attack attack) : this(points.ToArray(), attack) { }

        public IEnumerator<Point> GetEnumerator() => ((IEnumerable<Point>)Points).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
    }
}