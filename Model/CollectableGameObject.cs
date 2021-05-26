using System;
using System.Drawing;

namespace Game.Model
{
    public class CollectableGameObject : GameObject, ICollectable
    {
        private readonly Bitmap sprite;
        private readonly Action Action;
        public CollectableGameObject(GameModel game, Point position, string name, string description, Bitmap sprite, Action action) : base(game, position)
        {
            IsCollectable = true;
            this.sprite = sprite;
            Description = description;
            Name = name;
            this.Action = action;
            Drawer = new BasicDrawer(sprite, CollectImage);
        }

        public CollectableGameObject(GameModel game, int x, int y, string name, string description, Bitmap sprite, Action action) : this(game, new Point(x, y), name, description, sprite, action)
        {
        }

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => sprite;

        public void Collect()
        {
            Action();
            Remove();
        }

        private Bitmap CollectImage(BasicDrawer drawer) => new Bitmap(drawer.Sprite);
    }
}
