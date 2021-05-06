using System.Drawing;

namespace Game.Model
{
    public interface IPlaceable
    {
        public Point Position { get;}
        public int X { get; }
        public int Y { get; }
        public void PlaceItself(Level level);
    }
}