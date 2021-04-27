using System.Drawing;

namespace Game
{
    public class Kaba : Entity
    {
        private static readonly Bitmap Sprite = Properties.Resources.skull;
        public Kaba(Point position) : base(position)
        {
            Drawer = new BasicDrawer(
                Sprite,
                CollectImage,
                Color.Red
            );
            var pattern = new[] { new Size(1, -1), new Size(1, 0), new Size(1, 1) };
            Attack = new Attack(pattern, AttackType.Physical, 10, 2,false);
            Health = 150;
            IsPlayerControlled = true;
        }
        public Kaba(int x, int y) : this(new Point(x, y))
        {

        }

        public override BasicDrawer GetDrawer() => Drawer;

    }
}
