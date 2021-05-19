using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game.Model
{
    public class CollectableGameObject : GameObject,ICollectable
    {
        private readonly Bitmap sprite;
        private Action Action;
        public CollectableGameObject(GameModel game, Point position, string name, string description, Bitmap sprite, Action action) : base(game, position)
        {
            IsCollectable = true;
            this.sprite = sprite;
            Description = description;
            Name = name;
            Action = action;
            Drawer = new BasicDrawer(sprite, CollectImage);
        }

        public CollectableGameObject(GameModel game, int x, int y, string name, string description, Bitmap sprite, Action action) : this(game, new Point(x, y),name,description,sprite,action)
        {
        }

        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => sprite;

        public object Description { get; protected set; }
        public object Name { get; protected set; }

        public void Collect()
        {
            Action();
            Remove();
        }

        private Bitmap CollectImage(BasicDrawer drawer) => new Bitmap(drawer.Sprite);
    }
}
