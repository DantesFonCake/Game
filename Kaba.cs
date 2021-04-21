using System.Drawing;

namespace Game
{
    public class Kaba : Entity
    {

        public Kaba(int x, int y) : base(x, y)
        {
            Drawer = new BasicDrawer(
                Properties.Resources.skull,
                CollectImage
            );
            var pattern = new[] { new Size(1, -1), new Size(1, 0), new Size(1, 1) };
            Attack = new Attack(pattern, AttackType.Physical, 10);
            Health = 100;
            IsPlayerControlled = true;
        }

        public override BasicDrawer GetDrawer() => Drawer;

    }
}
