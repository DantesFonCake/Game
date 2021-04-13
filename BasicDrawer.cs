using System.Drawing;

namespace Game
{
    public class BasicDrawer
    {
        public delegate Bitmap ViewConstructor(BasicDrawer drawer);
        public readonly Bitmap Sprite;
        public ViewConstructor ConstructorView;

        public BasicDrawer(Bitmap sprite, ViewConstructor constructor)
        {
            Sprite = sprite;
            ConstructorView = constructor;
        }

        public virtual Bitmap GetView() => ConstructorView(this);

    }
}
