using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class Step
    {
        private readonly LinkedList<(object entity,ActionType type,Action action)> Actions = new LinkedList<(object, ActionType, Action)>();
        private readonly Dictionary<IAttacker, IEnumerable<Point>> attacksPreviews = new Dictionary<IAttacker, IEnumerable<Point>>();
        private readonly Dictionary<IMoveable, LinkedList<Point>> paths = new Dictionary<IMoveable, LinkedList<Point>>();
        public bool IsMovePossible(IMoveable moveable, Direction direction)
        {
            if (paths.ContainsKey(moveable))
            {
                var possiblePosition = paths[moveable].Last.Value + direction.GetOffsetFromDirection();
                return !paths[moveable].Contains(possiblePosition);
            }
            return true;
        }
        public bool TryGetEndPosition(IMoveable movable, out Point endPosition)
        {
            if (paths.TryGetValue(movable, out var position))
            {
                endPosition = position.Last.Value;
                return true;
            }
            endPosition = default;
            return false;
        }

        public void ScheduleAttack(Entity entity, Point position, Direction direction)
        {
            var originalPosition = entity.Position;
            if (!entity.Attack.IsRanged)
                Actions.AddLast((entity, ActionType.NotPreviewable, () => entity.MoveTo(position)));
            Actions.AddLast((entity,ActionType.Attack,() => entity.AttackPosition(position, direction)));
            if(!entity.Attack.IsRanged)
                Actions.AddLast((entity, ActionType.NotPreviewable, () => entity.MoveTo(originalPosition)));
            var attackPreview = entity.Attack.GetPositions(position, direction);
            if (!attacksPreviews.ContainsKey(entity))
                attacksPreviews[entity] = attackPreview;
            else
                attacksPreviews[entity].Concat(attackPreview).Distinct();
        }

        public void ScheduleMove(IMoveable entity, Direction direction)
        {
            Actions.AddLast((entity,ActionType.Move,() => entity.Move(direction)));
            if (paths.ContainsKey(entity))
                paths[entity].AddLast(paths[entity].Last.Value + Utils.GetOffsetFromDirection(direction));
            else
            {
                paths[entity] = new LinkedList<Point>();
                paths[entity].AddLast(entity.Position);
                paths[entity].AddLast(entity.Position + Utils.GetOffsetFromDirection(direction));
            }
        }

        public bool UndoScheduled()
        {
            if (Actions.Count == 0)
                return false;
            var (entity, type, action) = Actions.Last.Value;
            if (type == ActionType.Move)
                paths[entity as IMoveable].RemoveLast();
            else if (type == ActionType.Attack)
                attacksPreviews.Remove(entity as IAttacker);
            Actions.RemoveLast();
            return true;
        }

        public bool CommitStep()
        {
            if (Actions.Count == 0)
                return false;
            var (entity, type, action) = Actions.First.Value;
            action();
            if (type == ActionType.Move)
                paths[entity as IMoveable].RemoveFirst();
            else if (type == ActionType.Attack)
                attacksPreviews.Remove(entity as IAttacker);
            Actions.RemoveFirst();
            return true;
        }

        public IEnumerable<Point> GetPathPreview(IMoveable entity)
        {
            if (paths.ContainsKey(entity))
                foreach (var point in paths[entity])
                    yield return point;
        }

        public IEnumerable<Point> GetAttackPreview(IAttacker entity)
        {
            if (attacksPreviews.ContainsKey(entity))
                foreach (var point in attacksPreviews[entity])
                    yield return point;
        }
    }
}
