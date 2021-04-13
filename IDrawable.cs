using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public interface IDrawable
    {
        public IReadOnlyList<Bitmap> Variations { get; }
        public BasicDrawer GetDrawer();
    }
}
