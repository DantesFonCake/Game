using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace Game.Model
{
    public class GameModel
    {
        public Level CurrentLevel { get; private set; }
        public PlayerControlledEntity SelectedEntity { get; private set; }
        public Point? SelectedPosition => SelectedEntity.Ghost?.Position;
        public IEnumerable<Point> AttackPositions => SelectedEntity.GetPossibleAttackPositions(CurrentLevel, SelectedPosition.Value).Distinct();
        public KeyedAttack SelectedAttack { get; private set; }
        public bool ReadyToAttack
        {
            get => readyToAttack;
            private set
            {
                readyToAttack = value;
                if (!readyToAttack)
                    SelectedAttack = KeyedAttack.None;
            }
        }
        public bool IsAccessible { get; private set; } = true;
        public List<Action> ScheduledPath = new List<Action>();
        private readonly Timer Timer = new Timer();
        public PlayerScheduler PlayerScheduler;
        public Snake Snake;
        private bool readyToAttack;

        public GameModel(string[] level)
        {
            Timer.Interval = 1000 / 2.4;
            Timer.Elapsed += GameModelActionTimer_Tick;
            var (l, snake) = LevelCreator.FromLines(this, level);
            CurrentLevel = l;
            if (snake == null)
                return;
            Snake = snake;
            PlayerScheduler = new PlayerScheduler(snake);
            l.RecalculateVisionField(Snake.RecalculateVisionField(l));
        }


        private void GameModelActionTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!PlayerScheduler.Commit())
            {
                Timer.Stop();
                IsAccessible = true;
            }
        }

        public void TryUndoScheduled() => PlayerScheduler.Unschedule();

        public void TryScheduleAttack(Point position)
        {
            if (SelectedEntity != null)
                if (SelectedEntity.GetPossibleAttackPositions(CurrentLevel,SelectedPosition.Value).Contains(position))
                    if (SelectedEntity.Attack.IsRanged || CurrentLevel[position].IsPassable)
                    {

                        PlayerScheduler.AddAttack(SelectedEntity, position, SelectedPosition);
                    }
        }

        public void SelectAttackQ()
        {
            PrepareForAttack();
            if (ReadyToAttack)
            {
                SelectedEntity.SelectAttack(KeyedAttack.QAttack);
                SelectedAttack = KeyedAttack.QAttack;
            }
        }

        public void SelectAttackE()
        {
            PrepareForAttack();
            if (ReadyToAttack)
            {
                SelectedEntity.SelectAttack(KeyedAttack.EAttack);
                SelectedAttack = KeyedAttack.EAttack;
            }
        }

        private void PrepareForAttack() => ReadyToAttack = !ReadyToAttack && SelectedEntity != null && SelectedEntity.IsPlayerControlled && SelectedEntity.IsAlive;
        public void UnprepareForAttack() => ReadyToAttack = false;

        public void TrySheduleMove(Direction direction) => PlayerScheduler.AddMovement(x => Snake.MoveInDirection(x), direction);

        public void CommitStep(bool isImmediate = false)
        {
            ReadyToAttack = false;
            IsAccessible = false;
            if (isImmediate)
                while (PlayerScheduler.Commit()) ;
            else
                Timer.Start();
        }

        //TODO
        public void MoveToNextLevel(string[] lines)
        {
            var (l, snake) = LevelCreator.FromLines(this, lines);
            CurrentLevel = l;
            if (snake == null)
                return;
            Snake = snake;
            PlayerScheduler = new PlayerScheduler(snake);
            l.RecalculateVisionField(Snake.RecalculateVisionField(l));
        }

        public void SelectEntity(Entity entity)
        {
            if (IsAccessible)
                SelectedEntity = entity is PlayerControlledEntity ? entity as PlayerControlledEntity : null;
        }

        public void SelectEntity(Point point)
        {
            if (IsAccessible)
            {
                var s = PlayerScheduler.Ghosts.Keys.FirstOrDefault(x => x.Position == point);
                SelectedEntity = s != null
                    ? s.Entity
                    : CurrentLevel[point].GameObjects.Where(x => x is PlayerControlledEntity).Select(x => x as PlayerControlledEntity).FirstOrDefault();
            }
        }
    }
}

