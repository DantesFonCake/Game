using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    class Kaba : Entity
    { 
        public Kaba(int x, int y) : base(x, y)
        {
            CurrentVariation = 0;
            drawer = new BasicDrawer(
                new[] { Properties.Resources.skull },
                CollectImage
                );

        }

        public override BasicDrawer GetDrawer() => drawer;

    }
}
