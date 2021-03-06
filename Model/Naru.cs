using System.Drawing;

namespace Game.Model
{
    public class Naru : PlayerControlledEntity
    {
        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.Naru_Front_FaceOnly;

        public Naru(GameModel game, Point position) : base(game, position, 80, null)
        {
            Name = "Naru";
            Description = "Careless Assasin. Can do Physical and Poison damage";
            Drawer = new BasicDrawer(
                Sprite,
                CollectImage,
                Color.Pink
            );
            var pattern = new[] {new Size(-1,-1),new Size(-1,1),
            new Size(1,-1),new Size(1,1)};
            QAttack = new Attack(pattern, AttackType.Physical, 20, 3, false, 3);
            pattern = new[] { new Size(1, -1), new Size(1, 1) };
            EAttack = new Attack(pattern, AttackType.Poison, 25, 3, false, 2);
        }

        public Naru(GameModel game, int x, int y) : this(game, new Point(x, y))
        {

        }
    }
}
