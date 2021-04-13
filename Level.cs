using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class Level : IEnumerable<Tile>
    {
        public Tile[] Map;
        public int XSize { get; private set; }
        public int YSize { get; private set; }
        public Tile this[int x, int y]
        {
            get => Map[XSize * y + x];
            private set => Map[XSize * y + x] = value;
        }

        public bool InBounds(Point point) => bounds.Contains(point);
        public bool InBounds(int x, int y) => bounds.Contains(x, y);

        private Rectangle bounds;

        public Level(int xSize, int ySize)
        {
            XSize = xSize;
            YSize = ySize;
            bounds = new Rectangle(0, 0, XSize, YSize);
            Map = new Tile[xSize * ySize];
            for (var y = 0; y < ySize; y++)
                for (var x = 0; x < xSize; x++)
                    this[x, y] = new Tile(x, y, this);
        }

        public void PlaceObject(GameObject gameObject)
        {
            this[gameObject.X, gameObject.Y].GameObjects.Add(gameObject);
            if (gameObject is Entity entity)
                entity.Moved += Entity_Moved;
        }

        private void Entity_Moved(object sender, EntityMovementArgs e)
        {
            var entity = (Entity)sender;
            this[entity.X, entity.Y].GameObjects.Remove(entity);
            var nextX = entity.Position.X + e.DX;
            var nextY = entity.Position.Y + e.DY;
            if (InBounds(nextX, nextY) && this[nextX, nextY].GameObjects.TrueForAll((x) => !x.IsRigid))
            {
                entity.Position = new Point(nextX, nextY);
            }
            this[entity.X, entity.Y].GameObjects.Add(entity);
        }

        public IEnumerator<Tile> GetEnumerator() => ((IEnumerable<Tile>)Map).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Map.GetEnumerator();
    }
}