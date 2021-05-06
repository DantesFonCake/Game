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
        public Step Step;
        public Snake Snake;
        private bool readyToAttack;

        public GameModel(int x, int y, bool generateWalls, bool placeSnake, params GameObject[] objects)
        {

            Timer.Interval = 1000 / 2.4;
            Timer.Elapsed += GameModelActionTimer_Tick;
            Step = new Step();
            CurrentLevel = new Level(this, x, y);
            CurrentLevel.PlaceObject(new Stone(8, 9));
            CurrentLevel.PlaceObject(new Stone(8, 10));
            CurrentLevel.PlaceObject(new Stone(8, 11));
            foreach (var obj in objects)
            {
                CurrentLevel.PlaceObject(obj);
            }
            if (generateWalls)
                for (var yCoord = 0; yCoord < CurrentLevel.YSize; yCoord++)
                    for (var xCoord = 0; xCoord < CurrentLevel.XSize; xCoord++)
                        if (xCoord == 0 || xCoord == CurrentLevel.XSize - 1 || yCoord == 0 || yCoord == CurrentLevel.YSize - 1)
                            CurrentLevel.PlaceObject(new Stone(xCoord, yCoord));
            if (placeSnake)
            {
                Snake = new Snake(new Point(5, 5), new Point(5, 6), new Point(5, 7), new Point(5, 8));
                Snake.PlaceItself(CurrentLevel);
                CurrentLevel.RecalculateVisionField(Snake.RecalculateVisionField(CurrentLevel));
            }
        }


        private void GameModelActionTimer_Tick(object sender, ElapsedEventArgs e)
        {
            if (!Step.CommitStep())
            {
                Timer.Stop();
                IsAccessible = true;
            }
        }

        public GameModel(int x, int y) : this(x, y, true, true)
        {
        }

        public void TryUndoScheduled() => Step.UndoScheduled();

        public void TryScheduleAttack(Point position)
        {
            if (SelectedEntity != null)
                if (SelectedEntity.Attack.PossibleArea.Select(x => SelectedPosition + x).Contains(position))
                    if (SelectedEntity.Attack.IsRanged || CurrentLevel[position].IsPassable)
                        Step.ScheduleAttack(SelectedEntity, position, SelectedEntity.Direction, SelectedPosition.Value);
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

        public void TrySheduleMove(Direction direction)
        {
            if (Step.TryGetEndPosition(Snake.Kaba, out var position))
            {
                if (CurrentLevel.CanMove(position, direction) && Step.IsMovePossible(Snake.Kaba, direction))
                    Step.ScheduleMove(Snake, direction);
            }
            else if (CurrentLevel.CanMove(Snake.Position, direction))
                Step.ScheduleMove(Snake, direction);
        }

        public void CommitStep(bool isImmediate = false)
        {
            ReadyToAttack = false;
            IsAccessible = false;
            if (isImmediate)
                while (Step.CommitStep()) ;
            else
                Timer.Start();
        }

        //TODO
        public void MoveToNextLevel(int x, int y) => CurrentLevel = new Level(this, x, y);
        public void SelectEntity(Entity entity)
        {
            if (IsAccessible)
                SelectedEntity = entity is PlayerControlledEntity ? entity as PlayerControlledEntity : null;
        }

        public void SelectEntity(Point point)
        {
            if (IsAccessible)
            {
                var s = Step.Ghosts.Values.FirstOrDefault(x => x.Position == point);
                if (s != null)
                    SelectedEntity = s.Entity;
                else
                    SelectedEntity = CurrentLevel[point].GameObjects.Where(x => x is PlayerControlledEntity).Select(x => x as PlayerControlledEntity).FirstOrDefault();
            }
        }
    }
}

