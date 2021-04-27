using System;
using System.Drawing;

namespace Game
{
    public interface IMoveable: IPlaceable
    {
        //public event EventHandler<MovementArgs> Moved;

        void Move(Direction direction);
        public void MoveTo(Point newPosition,bool directionCheck);
    }
}