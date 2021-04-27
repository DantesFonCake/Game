using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Snake : IMoveable
    {
        public Kaba Kaba { get; private set; }
        public Hiro Hiro { get; private set; }
        public Point Position { get; private set; }
        public int X => Position.X;
        public int Y => Position.Y;

        public IEnumerable<Entity> Heroes 
        { 
            get 
            {
                yield return Kaba;
                yield return Hiro;
                //yield return Naru;
                //yield return Hana;
            } 
        }

        //public Naru Naru { get; private set; }
        //public Hana Hana {get; private set;}

        public Snake(Point kabaPosition, Point hiroPosition)
        {
            Position = kabaPosition;
            Kaba = new Kaba(kabaPosition);
            Hiro = new Hiro(hiroPosition);
        }

        //public event EventHandler<MovementArgs> Moved;

        public void MoveInDirection(Direction direction)
        {
            var previousPosition = Kaba.Position;
            Kaba.Move(direction);
            var oldHiroPosition = Hiro.Position;
            Hiro.MoveTo(previousPosition);
            
            Position = Kaba.Position;
        }

        public void PlaceItself(Level level)
        {
            level.PlaceObject(Kaba);
            level.PlaceObject(Hiro);
            //level.PlaceObject()
        }

        public void PlaceItself(Level level, Point positionForHead)
        {
            var hiroOffset = Kaba.Position - new Size(Hiro.Position);
            Kaba.Position = positionForHead;
            Hiro.Position = positionForHead + new Size(hiroOffset);


            PlaceItself(level);
        }

        public void Move(Direction direction) => MoveInDirection(direction);
        public void MoveTo(Point newPosition, bool directionCheck=false) => MoveInDirection(Utils.GetDirectionFromOffset(Position - new Size(newPosition)));
    }
}
