using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public abstract class Entity : GameObject
    {
        public event EventHandler<EntityMovementArgs> Moved;
        public BasicDrawer Drawer { get; protected set; }

        public Entity(int x, int y) : base(x, y)
        {
        }

        protected Entity(Point position) : base(position)
        {
        }

        public virtual void Move(Direction direction)
        {
            Direction = direction;
            OnMove(Utils.GetOffsetFromDirection(direction));
        }

        private void OnMove((int dX, int dY) offset)
        {
            if (Moved != null)
            {
                var args = new EntityMovementArgs(offset.dX, offset.dY);
                Moved.Invoke(this, args);
            }
        }

        protected Bitmap CollectImage(BasicDrawer drawer)
        {
            var rotate = Direction switch
            {
                Direction.Left => RotateFlipType.Rotate90FlipNone,
                Direction.Right => RotateFlipType.Rotate90FlipX,
                Direction.Up => RotateFlipType.RotateNoneFlipY,
                _ => RotateFlipType.RotateNoneFlipNone,
            };
            var image = new Bitmap(drawer.Sprite);
            image.RotateFlip(rotate);
            return image;
        }

    }
}
