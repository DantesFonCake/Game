using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public class AIStub
    {
        private readonly GameModel Game;


        public AIStub(GameModel game) => Game = game;


        public void ScheduleTowards(BasicEnemy enemy, IEnumerable<Point> positions)
        {
            var bestMove = enemy.MovePossibilities
              .Select(x => enemy.Position + x)
              .Where(x => Game.CurrentLevel.InBounds(x) && Game.CurrentLevel[x].IsPassable)
              .OrderBy(x => Game.CurrentLevel.BFSForLevel(x, positions.ToHashSet()).Count)
              .First();
            enemy.Scheduler.AddMovement(enemy.Move, (bestMove - new Size(enemy.Position)).GetDirectionFromOffset());
        }

        public bool TryScheduleAttack(BasicEnemy enemy, IEnumerable<Point> targets)
        {
            var rotations = new[] { Direction.Right, Direction.Up, Direction.Left, Direction.Down };
            var possibleAttackPositions = enemy.GetPossibleAttackPositions(Game.CurrentLevel, enemy.Position).OrderBy(x => x.DistanceTo(enemy.Position));
            var lookup = possibleAttackPositions.SelectMany(point => rotations.Select(rotation => ( point, rotation))).Distinct().ToDictionary(x => x, x => enemy.Attack.GetPositions(x.point, x.rotation));
            var posRotPairs = lookup.Where(x => x.Value.Intersect(targets).Any());
            if (!posRotPairs.Any())
                return false;
            var posRot = posRotPairs.First().Key;
            enemy.Scheduler.AddAttack(enemy, posRot.point,posRot.rotation);
            return true;
        }

    }
}
