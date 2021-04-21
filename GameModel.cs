using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class GameModel
    {
        public Level CurrentLevel { get; private set; }
        public Entity SelectedEntity { get; private set; }
        public bool ReadyToAttack { get; private set; }
        public bool IsAccessible { get; private set; } = true;
        public List<Action> SheduledPath = new List<Action>();
        public Step Step;

        public Kaba Kaba = new Kaba(5, 5);
        public Kaba Kaba2 = new Kaba(5, 6);
        //public Hiro Hiro;
        //public Naru Naru;
        //public Hana Hana;

        public GameModel(int x, int y, bool generateWalls, params GameObject[] objects)
        {
            Step = new Step(Kaba.Position);
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
        }
        public GameModel(int x, int y) : this(x, y, true)
        {
            CurrentLevel.PlaceObject(Kaba);
            CurrentLevel.PlaceObject(Kaba2);
        }

        public void PrepareForAttack() => ReadyToAttack = !ReadyToAttack && SelectedEntity != null && SelectedEntity.IsPlayerControlled;

        public void AttackPosition(Point position)
        {
            SelectedEntity?.AttackPosition(position);
            ReadyToAttack = false;
        }

        public void SheduleMove(Direction direction)
        {
            if (IsAccessible && CurrentLevel.CanMove(Step.Destination, direction))
                Step.AddDirection(direction);
        }

        public async void CommitStep()
        {
            IsAccessible = false;
            await Task.Run(() =>
            {
                foreach (var direction in Step.CommitStep())
                {
                    MoveHeroesInDirection(direction);
                    Thread.Sleep(700);
                }
            });
            IsAccessible = true;
        }


        public void MoveHeroesInDirection(Direction direction)
        {
            lock (Kaba)
                lock (Kaba2)
                {
                    var kabaOldPosition = Kaba.Position;
                    Kaba.Move(direction);
                    Kaba2.MoveTo(kabaOldPosition);

                }
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
