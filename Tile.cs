using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Tile : IDrawable
    {
        public IReadOnlyList<Bitmap> Variations => variations;
        public int x, y;
        private readonly bool IsVisible = true;
        private readonly bool IsUnknown = false;
        private readonly BasicDrawer drawer;
        public Level Level;
        private static readonly IReadOnlyList<Bitmap> variations = new[]
                {
            Properties.Resources.grass_1,
            Properties.Resources.grass_2,
            Properties.Resources.grass_3
        };

        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>(3);

        public Tile(int x, int y, Level level)
        {
            this.x = x;
            this.y = y;
            Level = level;
            var variantRandomiser = Utils.GetRandomInt();
            drawer = new BasicDrawer(
                variantRandomiser < 10 ? Variations[1] : variantRandomiser > 75 ? Variations[2] : Variations[0],
                CollectImage
            );
        }

        private Bitmap CollectImage(BasicDrawer drawer)
        {
            var mainImage = new Bitmap(drawer.Sprite);
            using (var g = Graphics.FromImage(mainImage))
            {
                foreach (var gameObject in GameObjects)
                {
                    g.DrawImage(gameObject.GetDrawer().GetView(), new Rectangle(Point.Empty, mainImage.Size));
                }
            }
            return mainImage;
        }

        public void ClearTile() => GameObjects = new List<GameObject>();
        public BasicDrawer GetDrawer() => drawer;
    }
}