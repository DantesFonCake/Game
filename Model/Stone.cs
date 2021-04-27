using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Stone : GameObject
    {
        private readonly BasicDrawer drawer;
        private static readonly IReadOnlyList<Bitmap> variations = new[]
        {
            Properties.Resources.stone_1,
            Properties.Resources.stone_2
        };

        public Stone(int x, int y) : base(x, y)
        {
            var variationRandomizer = Utils.GetRandomInt();
            drawer = new BasicDrawer(
                variationRandomizer > 75 ? variations[1] : variations[0],
                CollectImage);
        }

        private Bitmap CollectImage(BasicDrawer drawer) => new Bitmap(drawer.Sprite);

        public override BasicDrawer GetDrawer() => drawer;
    }
}
