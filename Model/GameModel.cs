using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace Game
{
    public class GameModel
    {
        public Level CurrentLevel { get; private set; }
        public Entity SelectedEntity { get; private set; }
        public bool ReadyToAttack { get; private set; }
        public bool IsAccessible { get; private set; } = true;
        public List<Action> ScheduledPath = new List<Action>();
        private readonly Timer Timer = new Timer();
        public Step Step;
        public Snake Snake;

        public GameModel(int x, int y, bool generateWalls, bool placeSnake, params GameObject[] objects)
        {

            Timer.Interval = 1000 / 2;
            Timer.Elapsed += GameModelActionTimer_Tick;
            Step = new Step();
            CurrentLevel = new Level(x, y);
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
                Snake = new Snake(new Point(5, 5), new Point(5, 6));
                Snake.PlaceItself(CurrentLevel);

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
            if (SelectedEntity.Attack.PossibleArea.Select(x => SelectedEntity.Position + x).Contains(position))
                Step.ScheduleAttack(SelectedEntity, position, SelectedEntity.Direction);
        }

        public void PrepareForAttack() => ReadyToAttack = !ReadyToAttack && SelectedEntity != null && SelectedEntity.IsPlayerControlled;

        public void AttackPosition(Point position)
        {
            SelectedEntity?.AttackPosition(position);
            ReadyToAttack = false;
        }

        public void TrySheduleMove(Direction direction)
        {
            if (Step.TryGetEndPosition(Snake, out var position))
            {
                if (CurrentLevel.CanMove(position, direction) && Step.IsMovePossible(Snake, direction))
                    Step.ScheduleMove(Snake, direction);
            }
            else if (CurrentLevel.CanMove(Snake.Position, direction))
                Step.ScheduleMove(Snake, direction);
        }

        public void CommitStep(bool isImmediate = false)
        {
            if (isImmediate)
                while (Step.CommitStep()) ;
            else
                Timer.Start();
            IsAccessible = false;
        }

        //TODO
        public void MoveToNextLevel(int x, int y) => CurrentLevel = new Level(x, y);
        public void SelectEntity(Point point)
        {
            if (IsAccessible)
                SelectedEntity = CurrentLevel[point].GameObjects.Where(x => x is Entity).Select(x => x as Entity).FirstOrDefault();
        }
    }
}
