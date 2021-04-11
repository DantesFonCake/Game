using System.Collections;
using System.Collections.Generic;

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

        public Level(int xSize, int ySize)
        {
            XSize = xSize;
            YSize = ySize;
            Map = new Tile[xSize * ySize];
            for (var y = 0; y < ySize; y++)
            {
                for (var x = 0; x < xSize; x++)
                {
                    this[x, y] = new Tile(x, y, this);
                    if (x == 0 || x == XSize - 1 || y == 0 || y == YSize - 1)
                        PlaceObject(new Stone(x, y));
                }
            }
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
            this[entity.X, entity.Y].ClearTile();
            var nextX = entity.Position.X + e.DX;
            var nextY = entity.Position.Y + e.DY;
            if (nextX < XSize && nextX >= 0 && this[nextX, nextY].GameObjects.TrueForAll((x) => !x.IsRigid))
                entity.X = nextX;
            if (nextY < YSize && nextY >= 0 && this[nextX, nextY].GameObjects.TrueForAll((x) => !x.IsRigid))
                entity.Y = nextY;
            this[entity.X, entity.Y].GameObjects.Add(entity);
        }

        public IEnumerator<Tile> GetEnumerator() => ((IEnumerable<Tile>)Map).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Map.GetEnumerator();
    }
}