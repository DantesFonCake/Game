using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class Attack
    {
        private readonly Size[] pattern;
        public IReadOnlyList<Size> PossibleArea { get => possibleArea; }
        public readonly AttackType Type;
        public readonly int Damage;
        public readonly int Range;
        public readonly bool IsRanged;
        private readonly Size[] possibleArea;

        public Attack(Size[] pattern, AttackType type, int damage,int range,bool isRanged)
        {
            this.pattern = pattern;
            Type = type;
            Damage = damage;
            Range = range;
            IsRanged = isRanged;
            possibleArea = Utils.GetRadius(range).ToArray();
        }

        public IEnumerable<Point> GetPositions(Point position, Direction direction) => pattern.Select(x => position + x).RotatePoints(direction, position);

        public IEnumerable<Point> GetPositions(Point position, double angle) => pattern.Select(x => position + x).RotatePoints(angle, position);
    }
}
