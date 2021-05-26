using System.Drawing;

namespace Game.Model
{
    public class Exit : GameObject
    {
        public Exit(GameModel game, Point position) : base(game, position)
        {
            Drawer = new BasicDrawer(Sprite, CollectImage);
            IsRigid = true;
        }

        public Exit(GameModel game, int x, int y) : this(game, new Point(x, y))
        {
        }

        public void Open() => IsRigid = false;

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => IsRigid ? ClosedSprite : OpenSprite;

        public Bitmap ClosedSprite => Properties.Resources.closed_gate;
        public Bitmap OpenSprite => Properties.Resources.opened_gate;

        private Bitmap CollectImage(BasicDrawer drawer) => Sprite;
    }
}
