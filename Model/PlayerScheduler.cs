using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class PlayerScheduler : Scheduler
    {
        public int MaxActionCount { get; protected set; } = 5;
        public int ActionCount { get; protected set; }
        public readonly LinkedList<Point> PathPreview = new LinkedList<Point>();
        public Dictionary<Ghost, LinkedList<Point>> Ghosts = new Dictionary<Ghost, LinkedList<Point>>();
        public Dictionary<PlayerControlledEntity, LinkedList<IEnumerable<Point>>> AttackPreview =
            new Dictionary<PlayerControlledEntity, LinkedList<IEnumerable<Point>>>();
        public Point LastPosition => PathPreview.Last.Value;
        public PlayerScheduler(Snake snake) : base()
        {
            foreach (var t in snake.Heroes)
            {
                Ghosts[t.Ghost] = new LinkedList<Point>();
                Ghosts[t.Ghost].AddLast(t.Ghost.Position);
                AttackPreview[t] = new LinkedList<IEnumerable<Point>>();
            }
            PathPreview.AddLast(snake.Position);
        }

        public bool ChangeMaxActionCount(int d)
        {
            var previous = MaxActionCount;
            if (previous + d <= 0)
                return false;
            MaxActionCount += d;
            return true;
        }

        #region Scheduling

        public override void AddAttack(Entity entity, Point position, Direction direction = Direction.None, Point? initialPosition = null)
        {
            if (ActionCount >= MaxActionCount)
                return;
            base.AddAttack(entity, position, direction, initialPosition);
            AttackPreview[entity as PlayerControlledEntity].AddLast(entity.Attack.GetPositions(position, direction));
            ActionCount++;
        }

        public override void AddMovement(Action<Direction> movement, Direction direction)
        {
            if (ActionCount >= MaxActionCount)
                return;
            base.AddMovement(movement, direction);
            var isFirst = true;
            var initialPosition = Point.Empty;
            foreach (var ghostPathPair in Ghosts)
            {
                if (isFirst)
                {
                    isFirst = false;
                    initialPosition = ghostPathPair.Key.Position;
                    ghostPathPair.Key.Move(direction);
                    ghostPathPair.Value.AddLast(ghostPathPair.Key.Position);
                    continue;
                }
                var additionalPosition = ghostPathPair.Key.Position;
                ghostPathPair.Key.Position = initialPosition;
                ghostPathPair.Value.AddLast(ghostPathPair.Key.Position);
                initialPosition = additionalPosition;
            }
            PathPreview.AddLast(PathPreview.Last.Value + direction.GetOffsetFromDirection());
            ActionCount++;
        }
        #endregion

        #region Commiting and Unscheduling
        public override bool Commit()
        {
            if (Actions.Count <= 0)
                return false;
            var act = Actions.First.Value;
            if (act.ActionType == ActionType.Move)
            {
                foreach (var gpPair in Ghosts)
                {
                    gpPair.Value.RemoveFirst();
                }
                PathPreview.RemoveFirst();
            }
            else if (act.ActionType == ActionType.Attack)
                AttackPreview[act.Target as PlayerControlledEntity].RemoveFirst();
            ActionCount -= ActionCount <= 0 ? 0 : 1;
            return base.Commit();
        }

        public override bool Unschedule()
        {
            if (Actions.Count <= 0)
                return false;
            var act = Actions.Last.Value;
            if (act.ActionType == ActionType.Move)
            {
                foreach (var gpPair in Ghosts)
                {
                    gpPair.Value.RemoveLast();
                    gpPair.Key.Position = gpPair.Value.Last.Value;
                }
                PathPreview.RemoveLast();
            }
            else if (act.ActionType == ActionType.Attack)
                AttackPreview[act.Target as PlayerControlledEntity].RemoveLast();
            ActionCount -= ActionCount <= 0 ? 0 : 1;
            return base.Unschedule();
        }
        #endregion
    }
}
