using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class BasicDrawer
    {
        public delegate Bitmap ViewConstructor(BasicDrawer drawer);
        public readonly Bitmap[] Variations;
        public ViewConstructor ConstructorView;

        public BasicDrawer(Bitmap[] variations, ViewConstructor constructor)
        {
            Variations = variations;
            ConstructorView = constructor;
        }

        public virtual Bitmap GetView() => ConstructorView(this);

    }
}
