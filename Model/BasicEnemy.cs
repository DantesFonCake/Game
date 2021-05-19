using System.Drawing;

namespace Game.Model
{
    public class BasicEnemy : Entity
    {
        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.skull;
        public Size[] MovePossibilities;
        public Scheduler Scheduler;

        public BasicEnemy(GameModel game, Point position) : base(game, position)
        {
            Attack = new Attack(new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1) }, AttackType.Physical, 50, 2, false);
            MovePossibilities = new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1), new Size(0, 0) };
            Drawer = new BasicDrawer(Sprite, CollectImage);
            Scheduler = new Scheduler();
        }

        public BasicEnemy(GameModel game, int x, int y) : this(game, new Point(x, y)) { }
    }
}
