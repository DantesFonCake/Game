using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class Attack
    {
        private readonly Size[] pattern;
        public AttackType Type { get; protected set; }
        public int Damage { get; protected set; }

        public Attack(Size[] pattern, AttackType type, int damage)
        {
            this.pattern = pattern;
            Type = type;
            Damage = damage;
        }

        public IEnumerable<Point> GetPositions(Point position, Direction direction) => pattern.Select(x => position + x).RotatePoints(direction, position);
    }
}
