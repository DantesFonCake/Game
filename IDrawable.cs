using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Game
{
    public interface IDrawable
    {
        public BasicDrawer GetDrawer();
    }
}
