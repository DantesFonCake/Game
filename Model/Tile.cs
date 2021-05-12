using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class Tile : IDrawable
    {
        public static IReadOnlyList<Bitmap> Variations { get; } = new[]
                {
            Properties.Resources.grass_1,
            Properties.Resources.grass_2,
            Properties.Resources.grass_3
        };
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
        public bool IsVisible { get; set; } = false;
        public bool IsUnknown { get; set; } = true;
        public bool IsPassable => GameObjects.TrueForAll(x => !x.IsRigid);
        public bool IsSeeThrough => GameObjects.TrueForAll(x => x.IsSeeThrough);
        public Level Level;
        private static Bitmap unknownTile;
        public List<GameObject> GameObjects { get; private set; } = new List<GameObject>(3);

        public BasicDrawer Drawer { get; protected set; }

        public Bitmap Sprite { get; private set; }

        public Tile(Point position, Level level)
        {
            Position = position;
            Level = level;
            var variantRandomiser = Utils.GetRandomInt();
            Sprite = variantRandomiser < 10 ? Variations[1] : variantRandomiser > 75 ? Variations[2] : Variations[0];
            Drawer = new BasicDrawer(
               Sprite,
                CollectImage
            );
            if (unknownTile == null)
            {
                var size = Drawer.Sprite.Size;
                var bitmap = new Bitmap(size.Width, size.Height);
                using var g = Graphics.FromImage(bitmap);
                g.FillRectangle(new SolidBrush(Color.Azure), new Rectangle(Point.Empty, size));
                unknownTile = bitmap;
            }
        }

        public Tile(int x, int y, Level level) : this(new Point(x, y), level)
        {
        }

        private Bitmap CollectImage(BasicDrawer drawer)
        {
            if (IsUnknown)
                return unknownTile;
            var mainImage = new Bitmap(drawer.Sprite);
            using (var g = Graphics.FromImage(mainImage))
            {
                lock (GameObjects)
                {
                    foreach (var gameObject in GameObjects)
                    {
                        if (gameObject is Entity entity)
                        {
                            if (entity.IsAlive && IsVisible)
                            {
                                g.DrawImage(entity.Drawer.GetView(), new Rectangle(Point.Empty, mainImage.Size));
                            }
                        }
                        else
                            g.DrawImage(gameObject.Drawer.GetView(), new Rectangle(Point.Empty, mainImage.Size));
                    }
                }
                if (!IsVisible)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(96, Color.Black)), new Rectangle(Point.Empty, mainImage.Size));
            }

            return mainImage;
        }

        public void ClearTile() => GameObjects = new List<GameObject>();
    }
}