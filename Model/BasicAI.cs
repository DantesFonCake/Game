using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public class BasicAI
    {
        private readonly GameModel Game;
        private List<BasicEnemy> aiControlled;
        private bool EnemyStepScheduled = false;
        private Dictionary<BasicEnemy, IEnumerable<Point>> lastSeenPositions = new Dictionary<BasicEnemy, IEnumerable<Point>>();

        public IReadOnlyList<BasicEnemy> AIControlled => aiControlled;
        public bool AnyAlive => aiControlled.Any(x => x.IsAlive);

        public BasicAI(GameModel game, List<BasicEnemy> aiControlled)
        {
            Game = game;
            this.aiControlled = aiControlled;
        }

        public bool CommitStep()
        {
            if (!EnemyStepScheduled)
            {
                foreach (var enemy in aiControlled)
                {
                    if (enemy.VisionFieldTask == null)
                        enemy.StartVisionFieldCalculation(Game.CurrentLevel);
                    enemy.VisionFieldTask.Wait();
                    var seenPositions = Game.Snake.Heroes.Where(x => x.IsAlive).Select(x => x.Position).Intersect(enemy.VisionFieldTask.Result);
                    if (seenPositions.Any())
                        lastSeenPositions[enemy] = seenPositions;
                    if (!TryScheduleAttack(Game.CurrentLevel, enemy, seenPositions))
                    {
                        var positions = lastSeenPositions.GetValueOrDefault(enemy, Enumerable.Empty<Point>());
                        ScheduleTowards(Game.CurrentLevel, enemy, positions);
                    }
                }
                EnemyStepScheduled = true;
            }
            var enemiesLeft = false;
            foreach (var enemy in aiControlled)
                enemiesLeft |= enemy.Scheduler.Commit();
            EnemyStepScheduled = enemiesLeft;
            return enemiesLeft;
        }

        public void ReplaceControlled(List<BasicEnemy> aiControlled)
        {
            this.aiControlled = aiControlled;
            lastSeenPositions = new Dictionary<BasicEnemy, IEnumerable<Point>>();
        }

        public bool ScheduleTowards(Level level, BasicEnemy enemy, IEnumerable<Point> positions)
        {
            var bestPath = BFSForLevel(level, enemy, positions.ToHashSet());
            if (bestPath.Count > 0)
            {
                enemy.Scheduler.AddMovement(enemy.Move, (new Size(bestPath[bestPath.Count-2]) - new Size(enemy.Position)).GetDirectionFromOffset());
                return true;
            }
            return false;
        }

        public bool TryScheduleAttack(Level level, BasicEnemy enemy, IEnumerable<Point> targets)
        {
            var rotations = new[] { Direction.Right, Direction.Up, Direction.Left, Direction.Down };
            var possibleAttackPositions = enemy.GetPossibleAttackPositions(level, enemy.Position).Where(x => level.InBounds(x) && level[x].IsPassable).OrderBy(x => x.DistanceTo(enemy.Position));
            var lookup = possibleAttackPositions.SelectMany(point => rotations.Select(rotation => (point, rotation))).Distinct().ToDictionary(x => x, x => enemy.Attack.GetPositions(x.point, x.rotation));
            var posRotPairs = lookup.Where(x => x.Value.Intersect(targets).Any());
            if (!posRotPairs.Any())
                return false;
            var posRot = posRotPairs.First().Key;
            enemy.Scheduler.AddAttack(enemy, posRot.point, posRot.rotation);
            return true;
        }

        public static List<Point> BFSForLevel(Level level, BasicEnemy enemy, HashSet<Point> destinations)
        {
            var start = enemy.Position;
            var queue = new Queue<Point>();
            queue.Enqueue(start);
            var track = new Dictionary<Point, SinglyLinkedList<Point>>
            {
                [start] = new SinglyLinkedList<Point>(start)
            };
            while (queue.Count != 0)
            {
                var node = queue.Dequeue();
                foreach (var neighbour in enemy.MovePossibilities.Select(x=>node+x))
                {

                    if (!level.InBounds(neighbour) || !level[neighbour].IsPassable && !destinations.Contains(neighbour) || track.ContainsKey(neighbour))
                        continue;
                    track[neighbour] = new SinglyLinkedList<Point>(neighbour, track[node]);
                    queue.Enqueue(neighbour);
                    if (destinations.Contains(neighbour))
                    {
                        return track[neighbour].ToList();
                    }
                }
            }
            return new List<Point>();
        }

    }
}
