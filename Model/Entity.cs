using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public abstract class Entity : GameObject, IMoveable, IAttacker
    {

        public bool IsPlayerControlled { get; protected set; }
        public string Name { get; protected set; }

        public Entity(GameModel game, int x, int y, int health = 100, Dictionary<AttackType, int> resistances = null) : this(game, new Point(x, y), health, resistances)
        {
        }

        public Entity(GameModel game, Point position, int health = 100, Dictionary<AttackType, int> resistances = null) : base(game, position)
        {
            this.resistances = resistances ?? new Dictionary<AttackType, int>();
            Health = health;
            initialHealth = health;
        }

        #region Movement and Rotation
        public event EventHandler<MovementArgs> Moved;
        internal void Rotate(Direction direction) => Direction = direction;
        public virtual void MoveTo(Point newPosition, bool directionCheck = false)
        {
            var offset = newPosition - new Size(Position);
            if (directionCheck)
            {
                var direction = Utils.GetDirectionFromOffset(offset);
                Direction = direction == Direction.None ? Direction : direction;
            }
            OnMove(offset);
        }

        public virtual void Move(Direction direction)
        {
            Direction = direction;
            OnMove(new Point(Utils.GetOffsetFromDirection(direction)));
        }

        private void OnMove(int dX, int dY) => Moved?.Invoke(this, new MovementArgs(dX, dY));
        private void OnMove(Point offset) => OnMove(offset.X, offset.Y);
        #endregion

        #region Health and Resistances
        protected readonly int initialHealth;
        public double Health { get; protected set; }
        public bool IsAlive => (int)Health > 0;
        protected readonly Dictionary<AttackType, int> resistances;
        public IReadOnlyDictionary<AttackType, int> Resistances { get; }
        public virtual void DealDamage(int damage, AttackType attackType)
        {
            if (IsAlive)
                Health -= damage * ((float)100 - resistances.GetValueOrDefault(attackType, 0)) / 100;

        }
        #endregion

        #region Attack
        public event EventHandler<AttackEventArgs> Attacked;
        public Attack Attack{ get; protected set;}
        public virtual void AttackPosition(Point position, Direction attackDirection = Direction.None)
        {
            if (Attack != null)
            {
                var positionsToAttack = Attack.GetPositions(position, attackDirection == Direction.None ? Direction : attackDirection);
                Attacked?.Invoke(this, new AttackEventArgs(positionsToAttack, Attack));
            }
        }
        #endregion

        protected virtual Bitmap CollectImage(BasicDrawer drawer)
        {
            var rotate = Direction switch
            {
                Direction.Left => RotateFlipType.Rotate90FlipNone,
                Direction.Right => RotateFlipType.Rotate90FlipX,
                Direction.Up => RotateFlipType.RotateNoneFlipY,
                _ => RotateFlipType.RotateNoneFlipNone,
            };
            var image = new Bitmap(drawer.Sprite);
            image.RotateFlip(rotate);
            using (var g = Graphics.FromImage(image))
            {
                g.FillRectangle(Brushes.Green, 2, image.Height - 5, (float)((image.Width - 4) * (Health / initialHealth)), 4);
            }
            return image;
        }

        
        public IEnumerable<Point> GetPossibleAttackPositions(Level level, Point position)
            => Attack != null ? level.GetAccesibleTiles(position, Attack.Range, false, x => !x.IsSeeThrough) : Enumerable.Empty<Point>();

        protected int rangeOfVision = 8;
        public IEnumerable<Point> RecalculateVisionField(Level level) => level.GetAccesibleTiles(Position, rangeOfVision, true, x => !x.IsSeeThrough);
    }
}
