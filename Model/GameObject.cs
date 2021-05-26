using System;
using System.Drawing;

namespace Game.Model
{
    public abstract class GameObject : IDrawable, IPlaceable
    {
        public GameModel Game;

        private bool isCollectable = false;
        private bool isRigid = true;

        public event EventHandler Removed;

        public string Name { get; protected set; } = "";
        public string Description { get; protected set; } = "";
        public bool IsSeeThrough { get; protected set; } = true;
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

        public GameObject(GameModel game, int x, int y) : this(game, new Point(x, y))
        {
        }

        public GameObject(GameModel game, Point position)
        {
            Position = position;
            Game = game;
        }

        public virtual void Remove() => Removed?.Invoke(this, EventArgs.Empty);

        public abstract BasicDrawer Drawer { get; protected set; }
        public abstract Bitmap Sprite { get; }
        public void PlaceItself(Level level) => level.PlaceObject(this);
    }
}