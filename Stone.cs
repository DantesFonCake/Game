using System;
using System.Drawing;

namespace Game
{
    internal class Stone : GameObject
    {
        private static readonly Random rnd = new Random();
        private readonly int currentVariation = rnd.Next(0, 100);
        private readonly BasicDrawer drawer;
        public Stone(int x, int y) : base(x, y) => drawer = new BasicDrawer(
                new[] {
                    Properties.Resources.stone_1,
                    Properties.Resources.stone_2
                },
                CollectImage);

        private Bitmap CollectImage(BasicDrawer drawer) => new Bitmap(currentVariation > 75 ? drawer.Variations[1] : drawer.Variations[0]);

        public override BasicDrawer GetDrawer() => drawer;
    }
}
