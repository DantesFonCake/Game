using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public abstract class Entity : GameObject
    {
        public event EventHandler<EntityMovementArgs> Moved;
        public BasicDrawer Drawer { get; protected set; }
        public event EventHandler<AttackEventArgs> Attacked;
        public Attack Attack { get; protected set; }
        public double Health { get; protected set; }
        public bool IsAlive => (int)Health > 0;
        public bool IsPlayerControlled { get; protected set; }
        public IReadOnlyDictionary<AttackType, int> Resistances { get; }

        private readonly Dictionary<AttackType, int> resistances;

        public Entity(int x, int y, int health = 100, Dictionary<AttackType, int> resistances = null) : base(x, y)
        {
            this.resistances = resistances ?? new Dictionary<AttackType, int>();
            Health = health;
        }

        public Entity(Point position, int health = 100, Dictionary<AttackType, int> resistances = null) : base(position)
        {
            this.resistances = resistances ?? new Dictionary<AttackType, int>();
            Health = health;
        }

        public virtual void MoveTo(Point newPosition)
        {
            var offset = newPosition - new Size(Position);
            Direction = Utils.GetDirectionFromOffset(offset);
            OnMove(offset);
        }

        public virtual void Move(Direction direction)
        {
            Direction = direction;
            var (dX, dY) = Utils.GetOffsetFromDirection(direction);
            OnMove(dX, dY);
        }

        private void OnMove(int dX, int dY) => Moved?.Invoke(this, new EntityMovementArgs(dX, dY));
        private void OnMove(Point offset) => OnMove(offset.X, offset.Y);

        public virtual void AttackPosition(Point position) => Attacked?.Invoke(this, new AttackEventArgs(Attack.GetPositions(position, Direction).Except(new[] { Position })));

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
            return image;
        }

        public virtual void DealDamage(int damage, AttackType attackType)
        {
            if (IsAlive)
                Health -= damage * ((float)100 - resistances.GetValueOrDefault(attackType, 0)) / 100;

        }
    }
}
