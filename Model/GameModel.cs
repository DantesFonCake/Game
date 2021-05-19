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
        public bool HasCollectable { get; set; } = false;
        public bool IsPlayerStep { get; protected set; } = true;
        private List<BasicEnemy> AIControlled = new List<BasicEnemy>();
        private readonly AIStub AI;
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
        public bool EnemyStepScheduled { get; private set; } = false;

        public List<Action> ScheduledPath = new List<Action>();
        private readonly Timer Timer = new Timer();
        public PlayerScheduler PlayerScheduler;
        public Snake Snake;
        private bool readyToAttack;

        public GameModel(string[] level)
        {
            Timer.Interval = 1000 / 2.4;
            Timer.Elapsed += GameModelActionTimer_Tick;
            AI = new AIStub(this);
            var l = LevelCreator.FromLines(this, level, out var snake, out AIControlled);
            CurrentLevel = l;
            if (snake == null)
                return;
            Snake = snake;
            PlayerScheduler = new PlayerScheduler(snake);
            l.RecalculateVisionField(Snake.RecalculateVisionField(l));
        }


        private void GameModelActionTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (IsPlayerStep)
            {
                IsPlayerStep = PlayerScheduler.Commit();
            }
            else
            {
                if (!EnemyStepScheduled)
                {
                    foreach (var enemy in AIControlled)
                    {
                        var positions = Snake.Heroes.Where(x => x.IsAlive).Select(x => x.Position);
                        if (!AI.TryScheduleAttack(enemy, positions))
                            AI.ScheduleTowards(enemy, positions);
                    }
                    EnemyStepScheduled = true;
                }
                var enemiesLeft = false;
                foreach (var enemy in AIControlled)
                {
                    enemiesLeft = enemiesLeft | enemy.Scheduler.Commit();
                }
                if (!enemiesLeft)
                {
                    EnemyStepScheduled = false;
                    IsPlayerStep = true;
                    Timer.Stop();
                    IsAccessible = true;
                }
            }
        }

        public void TryUndoScheduled() => PlayerScheduler.Unschedule();

        public void TryScheduleAttack(Point position, Direction direction)
        {
            if (SelectedEntity != null)
                if (SelectedEntity.GetPossibleAttackPositions(CurrentLevel, SelectedPosition.Value).Contains(position))
                    if (SelectedEntity.Attack.IsRanged || SelectedEntity.Position == position || CurrentLevel[position].IsPassable)
                    {

                        PlayerScheduler.AddAttack(SelectedEntity, position, direction, SelectedPosition);
                    }
        }

        public void SelectAttack(KeyedAttack attack)
        {
            if(attack==KeyedAttack.None)
            {
                ReadyToAttack = false;
                SelectedAttack = attack;
                return;
            }
            if (!ReadyToAttack)
            {
                PrepareForAttack();
            }
            if (ReadyToAttack)
            {
                if (SelectedAttack == attack)
                {
                    ReadyToAttack = false;
                    return;
                }
                if (SelectedEntity.Cooldowns.GetValueOrDefault(attack, 0) > 0)
                {
                    SelectedAttack = KeyedAttack.None;
                    return;
                }
                SelectedEntity.SelectAttack(attack);
                SelectedAttack = attack;
            }
        }

        private void PrepareForAttack() => ReadyToAttack = !ReadyToAttack && SelectedEntity != null && SelectedEntity.IsPlayerControlled && SelectedEntity.IsAlive;
        public void UnprepareForAttack() => ReadyToAttack = false;

        public void TrySheduleMove(Direction direction)
        {
            var nextPosition = PlayerScheduler.LastPosition + direction.GetOffsetFromDirection();
            if (CurrentLevel.InBounds(nextPosition) && CurrentLevel[nextPosition].IsPassable && !PlayerScheduler.Ghosts.Keys.Any(x => x.Position == nextPosition))
                PlayerScheduler.AddMovement(Snake.MoveInDirection, direction);
        }

        public void CommitStep(bool isImmediate = false)
        {
            ReadyToAttack = false;
            IsAccessible = false;
            if (isImmediate)
                while (PlayerScheduler.Commit()) ;
            else
                Timer.Start();
            foreach (var hero in Snake.Heroes)
                foreach (var key in hero.Cooldowns.Keys)
                    if (hero.Cooldowns[key] > 0)
                        hero.Cooldowns[key] -= 1;
        }

        //TODO
        public void MoveToNextLevel(string[] lines)
        {
            var l = LevelCreator.FromLines(this, lines, out var snake, out AIControlled);
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
            {
                if (entity == null)
                {
                    ReadyToAttack = false;
                    SelectedEntity = null;
                    return;
                }
                if (entity is PlayerControlledEntity e)
                {
                    if (e != SelectedEntity)
                        ReadyToAttack = false;
                    SelectedEntity = e;
                }
            }
        }

        public void SelectEntity(Point point)
        {
            if (IsAccessible)
            {
                var s = PlayerScheduler.Ghosts.Keys.FirstOrDefault(x => x.Position == point);
                var entity = s != null
                    ? s.Entity
                    : CurrentLevel[point].GameObjects.Where(x => x is PlayerControlledEntity).Select(x => x as PlayerControlledEntity).FirstOrDefault();
                SelectEntity(entity);
            }
        }
    }
}

