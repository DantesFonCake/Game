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

        public Tile this[Point point]
        {
            get => this[point.X, point.Y];
            private set => this[point.X, point.Y] = value;
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
            {
                entity.Moved += Entity_Moved;
                entity.Attacked += Entity_Attacked;
            }
        }

        public bool CanMove(Point position, Direction direction) => CanMove(position + Utils.GetOffsetFromDirection(direction));

        public bool CanMove(Point position) => InBounds(position) && this[position].GameObjects.TrueForAll(x => !x.IsRigid);

        private void Entity_Attacked(object sender, AttackEventArgs e)
        {
            var attack = ((Entity)sender).Attack;
            foreach (var point in e)
            {
                if (InBounds(point))
                {
                    lock (this[point])
                    {
                        foreach (var obj in this[point].GameObjects)
                            if (obj is Entity entity)
                            {
                                entity.DealDamage(attack.Damage, attack.Type);
                            }
                        this[point].GameObjects.RemoveAll(x => x is Entity e && !e.IsAlive);
                    }
                }
            }
        }

        private void Entity_Moved(object sender, MovementArgs e)
        {

            var entity = (Entity)sender;
            if (CanMove(entity.Position + new Size(e.DX, e.DY)))
            {
                lock (this[entity.Position])
                    this[entity.Position].GameObjects.Remove(entity);
                entity.Position = new Point(entity.X + e.DX, entity.Y + e.DY);
                lock (this[entity.Position])
                    this[entity.Position].GameObjects.Add(entity);
            }
        }

        public IEnumerator<Tile> GetEnumerator() => ((IEnumerable<Tile>)Map).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Map.GetEnumerator();
    }
}