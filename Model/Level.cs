using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game.Model
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
        public GameModel Game { get; protected set; }

        public Level(GameModel game, int xSize, int ySize)
        {
            Game = game;
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

        public bool CanMove(Point position) => InBounds(position) && this[position].IsPassable;

        private void Entity_Attacked(object sender, AttackEventArgs e)
        {
            var attack = e.Attack;
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
                lock (this[entity.Position].GameObjects)
                    this[entity.Position].GameObjects.Remove(entity);
                entity.Position = new Point(entity.X + e.DX, entity.Y + e.DY);
                lock (this[entity.Position].GameObjects)
                    this[entity.Position].GameObjects.Add(entity);
                if (Game == null || Game.Snake == null)
                    return;
                var newVisionField = Game.Snake.RecalculateVisionField(this);
                RecalculateVisionField(newVisionField);
            }
        }

        public IEnumerable<Point> GetAccesibleTiles(Point start, int radius, bool includeBorders, Func<Tile, bool> borderSelector)
        {
            yield return start;
            var ch = new HashSet<Point>();
            var angles = Enumerable.Range(0, (radius + 2) * 8).Select(x => x * Math.PI / (radius + 2) / 4);
            foreach (var angle in angles)
            {
                var sine = Math.Sin(angle);
                var cosine = Math.Cos(angle);
                var size = (X: cosine, Y: sine);
                for (double i = 1; i <= radius; i += 0.5)
                {
                    var position = start + new Size((int)Math.Truncate(size.X * i), (int)Math.Truncate(size.Y * i));
                    if (!InBounds(position) || ch.Contains(position))
                        continue;
                    if (borderSelector(this[position]))
                    {
                        if (includeBorders)
                            yield return position;
                        break;
                    }
                    yield return position;
                }
            }
        }

        public void RecalculateVisionField(HashSet<Point> newVisionField)
        {
            foreach (var position in newVisionField)
            {
                this[position].IsUnknown = false;
            }
            foreach (var tile in Map)
            {
                tile.IsVisible = newVisionField.Contains(tile.Position);
            }
        }

        public IEnumerator<Tile> GetEnumerator() => ((IEnumerable<Tile>)Map).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Map.GetEnumerator();

        public string[] Serialize()
        {
            var res = new string[YSize];
            for (var y = 0; y < YSize; y++)
            {
                var builder = new StringBuilder();
                for (var x = 0; x < XSize; x++)
                {
                    foreach (var obj in this[x, y].GameObjects)
                    {
                        builder.Append(obj.GetObjectType().UToString());
                        builder.Append(' ');
                    }
                    builder.Append(';');
                }
                res[y] = builder.ToString();
            }
            return res;
        }
    }
}