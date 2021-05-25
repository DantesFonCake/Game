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
                    var seenPositions = Game.Snake.Heroes.Where(x => x.IsAlive).Select(x => x.Position).Intersect(enemy.VisionField);
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
            {
                enemiesLeft |= enemy.Scheduler.Commit();
            }
            EnemyStepScheduled = enemiesLeft;
            return enemiesLeft;
        }

        public void ReplaceControlled(List<BasicEnemy> aiControlled)
        {
            this.aiControlled = aiControlled;
            lastSeenPositions = new Dictionary<BasicEnemy, IEnumerable<Point>>();
        }

        private void ScheduleTowards(Level level, BasicEnemy enemy, IEnumerable<Point> positions)
        {
            var bestMove = enemy.MovePossibilities
              .Select(x => enemy.Position + x)
              .Where(x => level.InBounds(x) && level[x].IsPassable)
              .OrderBy(x => level.BFSForLevel(x, positions.ToHashSet()).Count)
              .First();
            enemy.Scheduler.AddMovement(enemy.Move, (new Size(bestMove) - new Size(enemy.Position)).GetDirectionFromOffset());
        }

        private bool TryScheduleAttack(Level level, BasicEnemy enemy, IEnumerable<Point> targets)
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

    }
}
