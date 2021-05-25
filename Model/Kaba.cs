using System.Drawing;

namespace Game.Model
{
    public class Kaba : PlayerControlledEntity
    {
        public Kaba(GameModel game, Point position) : base(game, position, 150 , null)
        {
            Drawer = new BasicDrawer(
                Sprite,
                CollectImage,
                Color.Red
            );
            Name = "Kaba";
            var pattern = new[] { new Size(1, -1), new Size(1, 0), new Size(1, 1) };
            EAttack = new Attack(pattern, AttackType.Physical, 20, 2, false, 2);
            pattern = new[] { new Size(1, 0), new Size(2, 0) };
            QAttack = new Attack(pattern, AttackType.Physical, 28, 2, false, 2);
        }
        public Kaba(GameModel game, int x, int y) : this(game, new Point(x, y))
        {

        }

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.Kaba_Front_FaceOnly;

    }
}
