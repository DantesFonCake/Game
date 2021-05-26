using System.Drawing;

namespace Game.Model
{
    public class Ghost
    {
        public PlayerControlledEntity Entity { get; private set; }
        public Bitmap Sprite { get; private set; }
        public Point Position { get; set; }
        public Ghost(PlayerControlledEntity entity)
        {
            Entity = entity;
            Position = entity.Position;
            CollectImage(entity);
        }

        private void CollectImage(PlayerControlledEntity entity) => Sprite = entity.Sprite.SetOpacity(96);

        public void Move(Direction direction) => Position += direction.GetOffsetFromDirection();

        public void Rebound(PlayerControlledEntity entity)
        {
            Entity = entity;
            Rebound();
            CollectImage(entity);
        }
        public void Rebound() => Position = Entity.Position;
    }
}
