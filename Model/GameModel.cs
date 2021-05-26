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
        public bool HaveCollectable => Snake.Heroes
            .Where(x => x.IsAlive)
            .Any(x =>
                CurrentLevel[x.Position].GameObjects
                .Any(y => y.IsCollectable));
        public bool IsPlayerStep { get; protected set; } = true;

        private readonly IEnumerator<string[]> levelEnumerator = Levels.AllLevels.GetEnumerator();
        private readonly BasicAI AI;
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
        public PlayerScheduler PlayerScheduler;
        public Snake Snake;

        private bool readyToAttack;
        private bool IsStepInWork = false;
        private readonly Timer Timer = new Timer();

        public GameModel(bool initialize = false)
        {
            Timer.Interval = 1000 / 2.4;
            Timer.Elapsed += GameModelActionTimer_Tick;
            AI = new BasicAI(this, new List<BasicEnemy>());
            if (initialize)
                Initialize();
        }

        private void Initialize()
        {
            if (levelEnumerator.MoveNext())
                Initialize(levelEnumerator.Current);
        }

        private void Initialize(string[] levelLines)
        {
            var level = LevelCreator.FromLines(this, levelLines, out var aiControlled, out Snake snake);
            CurrentLevel = level;
            if (snake == null)
                return;
            Snake = snake;
            Snake.PlaceItself(level);
            PlayerScheduler = new PlayerScheduler(snake);
            level.RecalculateVisionField(Snake.GetVisionField(level));
        }

        public GameModel(string[] levelLines) : this() => Initialize(levelLines);


        private void GameModelActionTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!IsStepInWork)
            {
                IsStepInWork = true;
                if (IsPlayerStep)
                {
                    IsPlayerStep = PlayerScheduler.Commit();
                }
                else
                {
                    if (!AI.CommitStep())
                    {
                        IsPlayerStep = true;
                        Timer.Stop();
                        IsAccessible = true;
                        if (!AI.AnyAlive)
                            CurrentLevel.OpenGates();
                    }
                }
                IsStepInWork = false;
            }
        }

        public void TryUndoScheduled() => PlayerScheduler.Unschedule();

        public void TryScheduleAttack(Point position, Direction direction)
        {
            if (SelectedEntity != null
                && SelectedEntity.GetPossibleAttackPositions(CurrentLevel, SelectedPosition.Value).Contains(position)
                && (SelectedEntity.Attack.IsRanged || SelectedEntity.Position == position || CurrentLevel[position].IsPassable))
                PlayerScheduler.AddAttack(SelectedEntity, position, direction, SelectedPosition);
        }

        public void SelectAttack(KeyedAttack attack)
        {
            if (attack == KeyedAttack.None)
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
                    ReadyToAttack = false;
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
            foreach (var enemy in AI.AIControlled.Where(x => x.IsAlive))
                enemy.StartVisionFieldCalculation(CurrentLevel);
            if (isImmediate)
            {
                while (PlayerScheduler.Commit()) ;
                while (AI.CommitStep()) ;
            }
            else
                Timer.Start();
            foreach (var hero in Snake.Heroes)
                hero.ReduceCooldowns();
        }


        public void MoveToNextLevel(string[] lines)
        {
            IsAccessible = false;
            ReadyToAttack = false;
            Timer.Stop();
            var level = LevelCreator.FromLines(this, lines, out var aiControlled, out Tuple<Point, Point, Point, Point> snakePositions);
            AI.ReplaceControlled(aiControlled);
            CurrentLevel = level;
            if (Snake != null && snakePositions != null)
            {
                Snake.PlaceItself(level, snakePositions);
                level.RecalculateVisionField(Snake.GetVisionField(level));
            }
            IsPlayerStep = true;
            IsAccessible = true;
        }

        public void MoveToNextLevel()
        {
            if (levelEnumerator.MoveNext())
                MoveToNextLevel(levelEnumerator.Current);
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
            if (CurrentLevel.InBounds(point))
                if (IsAccessible)
                {
                    var s = PlayerScheduler.Ghosts.Keys.FirstOrDefault(x => x.Position == point);
                    var entity = s != null
                        ? s.Entity
                        : CurrentLevel[point].GameObjects
                            .Where(x => x is PlayerControlledEntity)
                            .Select(x => x as PlayerControlledEntity)
                            .FirstOrDefault();
                    SelectEntity(entity);
                }
        }
    }
}

