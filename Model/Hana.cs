using System.Drawing;

namespace Game.Model
{
    public class Hana : PlayerControlledEntity
    {
        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.Hana_Front_FaceOnly;

        public Hana(GameModel game, Point position) : base(game, position, 80, null)
        {
            Name = "Hana";
            Description = "Quick tempered Mage. Does Fire and Ice damage";
            Drawer = new BasicDrawer(
                Sprite,
                CollectImage,
                Color.BlueViolet
            );
            var pattern = new[] {new Size(1,-1),new Size(1,0),new Size(1,1),
                new Size(2,-2),new Size(2,-1),new Size(2,0),new Size(2,1),new Size(2,2)};
            QAttack = new Attack(pattern, AttackType.Fire, 15, 1, false, 3);
            pattern = new[] {new Size(-1,0),
                new Size(0,-1),new Size(0,0),new Size(0,1),
                new Size(1,0)
            };
            EAttack = new Attack(pattern, AttackType.Ice, 25, 5, true, 2);
        }

        public Hana(GameModel game, int x, int y) : this(game, new Point(x, y))
        {

        }
    }
}
