using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class Attack
    {
        private readonly Size[] pattern;
        public readonly AttackType Type;
        public int Damage { get; protected set; }
        public readonly int Range;
        public readonly bool IsRanged;
        public readonly int Cooldown;

        public Attack(Size[] pattern, AttackType type, int damage, int range, bool isRanged, int cooldown = 0)
        {
            this.pattern = pattern;
            Type = type;
            Damage = damage;
            Range = range;
            IsRanged = isRanged;
            Cooldown = cooldown;
        }

        public IEnumerable<Point> GetPositions(Point position, Direction direction) => pattern.Select(x => position + x).RotatePoints(direction, position);

        public IEnumerable<Point> GetPositions(Point position, double angle) => pattern.Select(x => position + x).RotatePoints(angle, position);

        public bool ChangeDamage(int d)
        {
            var previous = Damage;
            if (previous + d <= 0)
                return false;
            Damage += d;
            return true;
        }
    }
}
