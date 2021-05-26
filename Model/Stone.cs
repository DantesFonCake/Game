using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class Stone : GameObject
    {
        private static readonly IReadOnlyList<Bitmap> variations = new[]
        {
            Properties.Resources.stone_1,
            Properties.Resources.stone_2
        };

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite { get; }

        public Stone(GameModel game, int x, int y) : base(game, x, y)
        {
            IsSeeThrough = false;
            var variationRandomizer = Utils.GetRandomInt();
            Sprite = variationRandomizer > 75 ? variations[1] : variations[0];
            Drawer = new BasicDrawer(
                Sprite,
                CollectImage);
        }

        private Bitmap CollectImage(BasicDrawer drawer) => new Bitmap(drawer.Sprite);
    }
}
