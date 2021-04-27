using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Hiro : Entity
    {
        static readonly Bitmap Sprite = Properties.Resources.skull;
        readonly BasicDrawer drawer;
        public Hiro(Point position) : base(position)
        {
            drawer = new BasicDrawer(Sprite, CollectImage,Color.Blue);
            Health = 100;
            IsPlayerControlled = true;
            var pattern = new[] { new Size(-1, -1), new Size(0, 0), new Size(1, -1), new Size(1, 1), new Size(-1, 1) };
            Attack = new Attack(pattern, AttackType.Physical, 1000, 6,true);
        }

        public Hiro(int x, int y) : this(new Point(x, y))
        {
        }

        public override BasicDrawer GetDrawer() => drawer;
    }
}