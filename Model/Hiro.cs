using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class Hiro : PlayerControlledEntity
    {
        public Hiro(Point position) : base(position,100,null)
        {
            Drawer = new BasicDrawer(Sprite, CollectImage,Color.Blue);
            Name = "Hiro";
            var pattern = new[] { new Size(-1, -1), new Size(0, 0), new Size(1, -1), new Size(1, 1), new Size(-1, 1) };
            EAttack = new Attack(pattern, AttackType.Physical, 10, 6,true);
            pattern = new[] { new Size(0, 0) };
            QAttack = new Attack(pattern, AttackType.Physical, 30, 7, true);
            rangeOfVision = 9;
        }

        public Hiro(int x, int y) : this(new Point(x, y))
        {
        }

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.Hiro_Front_FaceOnly;
    }
}