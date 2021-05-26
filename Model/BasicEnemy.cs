using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Model
{
    public class BasicEnemy : Entity
    {
        public override BasicDrawer Drawer { get; protected set; }

        public override Bitmap Sprite => Properties.Resources.skull;
        public Task<HashSet<Point>> VisionFieldTask { get; protected set; }
        public IReadOnlyList<Size> MovePossibilities => movePossibilities;
        public Scheduler Scheduler { get; protected set; }
        protected Size[] movePossibilities;

        public BasicEnemy(GameModel game, Point position) : base(game, position)
        {
            Attack = new Attack(new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1) }, AttackType.Physical, 50, 2, false);
            movePossibilities = new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1), new Size(0, 0) };
            Drawer = new BasicDrawer(Sprite, CollectImage);
            Scheduler = new Scheduler();
            Name = "Skully";
            Description = "A lonely creepy skeleton";
        }

        public BasicEnemy(GameModel game, int x, int y) : this(game, new Point(x, y)) { }

        public void StartVisionFieldCalculation(Level level)
            => VisionFieldTask = Task.Run(() => RecalculateVisionField(level).ToHashSet());
    }
}
