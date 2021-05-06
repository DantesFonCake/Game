using System.Drawing;

namespace Game
{
    public interface IDrawable
    {
        public BasicDrawer Drawer { get; }
        public Bitmap Sprite { get; }
    }
}
