using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game.Model
{
    public class BasicEnemy : Entity
    {
        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.skull;
        public Scheduler Scheduler;

        public BasicEnemy(GameModel game,Point position) : base(game, position)
        {
            Drawer = new BasicDrawer(Sprite, CollectImage);
            Scheduler = new Scheduler();
        }

        public BasicEnemy(GameModel game,int x,int y):this(game,new Point(x, y)) { }
    }
}
