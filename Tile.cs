using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Tile : IDrawable
    {
        public IReadOnlyList<Bitmap> Variations => variations;
        public Point Position { get; set; }
        public int X
        {
            get => Position.X;
            set => Position = new Point(value, Y);
        }
        public int Y
        {
            get => Position.Y;
            set => Position = new Point(X, value);
        }
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

        public Tile(Point position, Level level)
        {
            Position = position;
            Level = level;
            var variantRandomiser = Utils.GetRandomInt();
            drawer = new BasicDrawer(
                variantRandomiser < 10 ? Variations[1] : variantRandomiser > 75 ? Variations[2] : Variations[0],
                CollectImage
            );
        }

        public Tile(int x, int y, Level level) : this(new Point(x, y), level)
        {
        }

        private Bitmap CollectImage(BasicDrawer drawer)
        {
            if (IsUnknown)
                return null;//TODO there will be sprite for unknown tile
            var mainImage = new Bitmap(drawer.Sprite);
            if (IsVisible)
                using (var g = Graphics.FromImage(mainImage))
                {
                    foreach (var gameObject in GameObjects)
                    {
                        if (gameObject is Entity entity)
                        {
                            if (entity.IsAlive)
                            {
                                g.DrawImage(entity.GetDrawer().GetView(), new Rectangle(Point.Empty, mainImage.Size));
                                g.FillRectangle(Brushes.Green, 2, mainImage.Height - 9, (float)((double)(mainImage.Width - 4) / 100 * entity.Health), 8);
                            }
                        }
                        else
                            g.DrawImage(gameObject.GetDrawer().GetView(), new Rectangle(Point.Empty, mainImage.Size));
                    }
                }
            return mainImage;
        }

        public void ClearTile() => GameObjects = new List<GameObject>();
        public BasicDrawer GetDrawer() => drawer;
    }
}