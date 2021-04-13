using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Kaba : Entity
    {
        public override IReadOnlyList<Bitmap> Variations => variations;

        private static readonly IReadOnlyList<Bitmap> variations = new[] { Properties.Resources.skull };

        public Kaba(int x, int y) : base(x, y) => Drawer = new BasicDrawer(
                Variations[0],
                CollectImage
                );

        public override BasicDrawer GetDrawer() => Drawer;

    }
}
