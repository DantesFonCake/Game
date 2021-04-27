using System.Drawing;

namespace Game
{
    public class BasicDrawer
    {
        public delegate Bitmap ViewConstructor(BasicDrawer drawer);
        public readonly Bitmap Sprite;
        public readonly Color SpecificColor;
        public ViewConstructor ConstructorView;

        public BasicDrawer(Bitmap sprite, ViewConstructor constructor, Color? objectSpecificColor=null)
        {
            Sprite = sprite;
            ConstructorView = constructor;
            if (objectSpecificColor != null)
                SpecificColor = objectSpecificColor.Value;
        }

        public virtual Bitmap GetView() => ConstructorView(this);

    }
}
