using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game
{
    public class Attack
    {
        Size[] pattern;
        public AttackType Type { get; protected set; }
        public int Damage { get; protected set; }

        public Attack(Size[] pattern, AttackType type, int damage)
        {
            this.pattern = pattern;
            Type = type;
            Damage = damage;
        }

        public IEnumerable<Point> GetPositions(Point position, Direction direction)
        {
            return pattern.Select(x => position + x).RotatePoints(direction, position);
        }
    }
}
