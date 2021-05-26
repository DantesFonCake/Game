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
        HashSet<Exit> exits = new HashSet<Exit>();
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
            this[gameObject.X, gameObject.Y].AddObject(gameObject);
            gameObject.Removed += GameObject_Removed;
            if (gameObject is Exit exit)
                exits.Add(exit);
            if (gameObject is Entity entity)
            {
                entity.Moved += Entity_Moved;
                entity.Attacked += Entity_Attacked;
            }
        }

        private void GameObject_Removed(object sender, EventArgs e)
        {
            var obj = (GameObject)sender;
            if (this[obj.Position].RemoveObject(obj))
            {
                obj.Removed -= GameObject_Removed;
                if (obj is Entity entity)
                {
                    entity.Moved -= Entity_Moved;
                    entity.Attacked -= Entity_Attacked;
                }
            }
        }

        public bool CanMove(Point position, Direction direction) => CanMove(position + Utils.GetOffsetFromDirection(direction));

        public bool CanMove(Point position) => InBounds(position) && this[position].IsPassable;

        private void Entity_Attacked(object sender, AttackEventArgs e)
        {
            if (e.Handled)
                return;
            e.Handled = true;
            var attack = e.Attack;
            foreach (var point in e)
            {
                if (InBounds(point))
                {
                    foreach (var obj in this[point].GameObjects)
                        if (obj is Entity entity)
                            entity.DealDamage(attack.Damage, attack.Type);
                }
            }
        }

        private void Entity_Moved(object sender, MovementArgs e)
        {
            if (e.Handled)
                return;
            e.Handled = true;
            var entity = (Entity)sender;
            if (CanMove(entity.Position + new Size(e.DX, e.DY)))
            {
                this[entity.Position].RemoveObject(entity);
                entity.Position = new Point(entity.X + e.DX, entity.Y + e.DY);
                this[entity.Position].AddObject(entity);
                if (Game == null || Game.Snake == null)
                    return;
                if (exits.Select(x=>x.Position).Contains(Game.Snake.Position))
                {
                    Game.MoveToNextLevel();
                    return;
                }
                var newVisionField = Game.Snake.GetVisionField(this);
                RecalculateVisionField(newVisionField);
            }
        }

        public void OpenGates()
        {
            foreach (var exit in exits)
                exit.Open();
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
                var (X, Y) = (cosine, sine);
                for (double i = 1; i <= radius; i += 0.5)
                {
                    var position = start + new Size((int)Math.Truncate(X * i), (int)Math.Truncate(Y * i));
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