using System;
using System.Drawing;

namespace Game.Model
{
    public interface IMoveable: IPlaceable
    {
        void Move(Direction direction);
        public void MoveTo(Point newPosition,bool directionCheck);
    }
}