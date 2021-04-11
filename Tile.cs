using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Tile:IDrawable
    {
        static Random rnd = new Random();
        public int x, y;
        bool IsVisible = true;
        bool IsUnknown=false;
        private BasicDrawer drawer;
        public Level Level;
        private List<GameObject> gameObjects = new List<GameObject>(3);
        private int currentVariation = rnd.Next(0, 100);
        public List<GameObject> GameObjects
        {
            get => gameObjects;
            private set
            {
                gameObjects = value;
            }
        }

        public Tile(int x, int y, Level level)
        {
            this.x = x;
            this.y = y;
            Level = level;
            drawer = new BasicDrawer(
                new[]
                {
                    Properties.Resources.grass_1,
                    Properties.Resources.grass_2,
                    Properties.Resources.grass_3
                },
                CollectImage
            );
        }

        private Bitmap CollectImage(BasicDrawer drawer)
        {
            var mainImage = new Bitmap(currentVariation < 10 ? drawer.Variations[1] : currentVariation > 75 ? drawer.Variations[2] : drawer.Variations[0]);
            using(var g = Graphics.FromImage(mainImage))
            {
                foreach (var gameObject in gameObjects)
                {
                    g.DrawImage(gameObject.GetDrawer().GetView(), new Rectangle(Point.Empty, mainImage.Size));
                }
            }
            return mainImage;
        }

        public void ClearTile() => gameObjects = new List<GameObject>();
        public BasicDrawer GetDrawer() => drawer;
    }
}