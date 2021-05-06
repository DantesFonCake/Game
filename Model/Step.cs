using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public partial class Step
    {
        private readonly LinkedList<(Entity entity, ActionType type, Action action)> actions = new LinkedList<(Entity, ActionType, Action)>();
        private readonly Dictionary<Entity, LinkedList<IEnumerable<Point>>> attacksPreviews = new Dictionary<Entity, LinkedList<IEnumerable<Point>>>();
        private readonly Dictionary<Entity, LinkedList<Point>> paths = new Dictionary<Entity, LinkedList<Point>>();
        private readonly Dictionary<PlayerControlledEntity, Ghost> ghosts = new Dictionary<PlayerControlledEntity, Ghost>();

        public IReadOnlyDictionary<PlayerControlledEntity, Ghost> Ghosts => ghosts;

        public bool IsMovePossible(Entity moveable, Direction direction)
        {
            if (paths.ContainsKey(moveable))
            {
                var possiblePosition = paths[moveable].Last.Value + direction.GetOffsetFromDirection();
                return !paths[moveable].Contains(possiblePosition);
            }
            return true;
        }
        public bool TryGetEndPosition(Entity movable, out Point endPosition)
        {
            if (paths.TryGetValue(movable, out var position))
            {
                endPosition = position.Last.Value;
                return true;
            }
            endPosition = default;
            return false;
        }

        public void ScheduleAttack(Entity entity, Point position, Direction direction, Point initialPosition)
        {
            if (!entity.Attack.IsRanged)
                actions.AddLast((entity, ActionType.NotPreviewable, (Action)(() =>
                {
                    entity.MoveTo(position);
                    entity.Rotate(direction);
                }
                )));
            actions.AddLast((entity, ActionType.Attack, () => entity.AttackPosition(position, direction)));
            var originalDirection = entity.Direction;
            if (!entity.Attack.IsRanged)
                actions.AddLast((entity, ActionType.NotPreviewable, (Action)(() =>
                {
                    entity.MoveTo(initialPosition);
                    entity.Rotate(originalDirection);
                }
                )));
            var attackPreview = entity.Attack.GetPositions(position, direction);
            if (!attacksPreviews.ContainsKey(entity))
                attacksPreviews[entity] = new LinkedList<IEnumerable<Point>>();
            attacksPreviews[entity].AddLast(attackPreview);

        }

        public void ScheduleMove(Snake snake, Direction direction)
        {
            var d = direction.Copy();
            Action action = () =>
              {
                  snake.MoveInDirection(d);
              };
            var head = snake.Heroes.First();
            actions.AddLast((head, ActionType.Move, action));
            AddPathPreview(head, direction);
            if (!Ghosts.ContainsKey(head))
                ghosts[head] = head.Ghost;
            var initialPosition = ghosts[head].Position;
            ghosts[head].Move(direction);
            foreach (var entity in snake.Heroes.Skip(1))
            {
                if (!Ghosts.ContainsKey(entity))
                    ghosts[entity] = entity.Ghost;
                direction = Utils.GetDirectionFromOffset(initialPosition - new Size(ghosts[entity].Position));
                initialPosition = ghosts[entity].Position;
                ghosts[entity].Move(direction);
            }
        }

        private void AddPathPreview(Entity entity, Direction direction)
        {
            if (paths.ContainsKey(entity))
                paths[entity].AddLast(paths[entity].Last.Value + Utils.GetOffsetFromDirection(direction));
            else
            {
                paths[entity] = new LinkedList<Point>();
                paths[entity].AddLast(entity.Position);
                paths[entity].AddLast(entity.Position + Utils.GetOffsetFromDirection(direction));
            }
        }
        public void ScheduleMove(Entity entity, Direction direction) => actions.AddLast((entity, ActionType.Move, () => entity.Move(direction)));//AddPathPreview(entity, direction);

        public bool UndoScheduled()
        {
            if (actions.Count == 0)
                return false;
            var (entity, type, action) = actions.Last.Value;
            if (type == ActionType.Move)
                paths[entity].RemoveLast();
            else if (type == ActionType.Attack)
                attacksPreviews[entity].RemoveLast();
            actions.RemoveLast();
            return true;
        }

        public bool CommitStep()
        {
            if (actions.Count == 0)
                return false;
            var (entity, type, action) = actions.First.Value;
            action();
            if (type == ActionType.Move)
                paths[entity].RemoveFirst();
            else if (type == ActionType.Attack)
                attacksPreviews[entity].RemoveFirst();
            actions.RemoveFirst();
            return true;
        }

        public IEnumerable<Point> GetPathPreview(Entity entity)
        {
            if (paths.ContainsKey(entity))
                foreach (var point in paths[entity])
                    yield return point;
        }

        public IEnumerable<Point> GetAttackPreview(Entity entity)
        {
            if (attacksPreviews.ContainsKey(entity))
                foreach (var previews in attacksPreviews[entity])
                    foreach (var point in previews)
                        yield return point;
        }
    }
}
