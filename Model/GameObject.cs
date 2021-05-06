using System.Drawing;

namespace Game.Model
{
    public abstract class GameObject : IDrawable, IPlaceable
    {
        private bool isCollectable = false;
        private bool isRigid = true;
        public Direction Direction { get; protected set; } = Direction.Down;
        public Point Position { get; set; }
        public int X
        {
            get => Position.X;
            set => Position = new Point(value, Y);
        }
        public int Y
        {
            get => Position.Y;
            set => Position = new Point(X, value);
        }
        public bool IsRigid
        {
            get => isRigid;
            protected set
            {
                isRigid = value;
                if (isRigid)
                    isCollectable = false;
            }
        }

        public bool IsCollectable
        {
            get => isCollectable;
            protected set
            {
                isCollectable = value;
                if (isCollectable)
                    isRigid = false;
            }
        }

        public GameObject(int x, int y) : this(new Point(x, y))
        {
        }

        public GameObject(Point position) => Position = position;

        public abstract BasicDrawer Drawer { get; protected set; }
        public abstract Bitmap Sprite { get; }
        public void PlaceItself(Level level) => level.PlaceObject(this);
    }
}