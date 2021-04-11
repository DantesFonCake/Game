using System;
using System.Drawing;

namespace Game
{
    public abstract class Entity : GameObject
    {        
        public event EventHandler<EntityMovementArgs> Moved;
        public BasicDrawer drawer { get; protected set; }
        protected int CurrentVariation = 0;
        public Entity(int x,int y) : base(x,y)
        {
        }

        public void Move(Direction direction)
        {
            var dX = 0;
            var dY = 0;
            Direction = direction;
            switch (direction)
            {
                case Direction.Left:
                    dX = -1;
                    break;
                case Direction.Right:
                    dX = 1;
                    break;
                case Direction.Up:
                    dY = -1;
                    break;
                case Direction.Down:
                    dY = 1;
                    break;
            }
            OnMove(dX, dY);
        }

        private void OnMove(int dX, int dY)
        {
            if (Moved != null) 
            {
                var args = new EntityMovementArgs(dX, dY);
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
            var image = new Bitmap(drawer.Variations[CurrentVariation]);
            image.RotateFlip(rotate);
            return image;
        }

    }
}
