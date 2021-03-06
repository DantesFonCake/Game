using System.Drawing;

namespace Game.Model
{
    public class Hiro : PlayerControlledEntity
    {
        public Hiro(GameModel game, Point position) : base(game, position, 100, null)
        {
            Drawer = new BasicDrawer(Sprite, CollectImage, Color.Blue);
            Name = "Hiro";
            Description = "Calm Archer. All attack are Physical";
            var pattern = new[] { new Size(-1, -1), new Size(0, 0), new Size(1, -1), new Size(1, 1), new Size(-1, 1) };
            EAttack = new Attack(pattern, AttackType.Physical, 15, 6, true, 2);
            pattern = new[] { new Size(0, 0) };
            QAttack = new Attack(pattern, AttackType.Physical, 30, 7, true, 3);
            rangeOfVision = 9;
        }

        public Hiro(GameModel game, int x, int y) : this(game, new Point(x, y))
        {
        }

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.Hiro_Front_FaceOnly;
    }
}