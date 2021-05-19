using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Model
{
    public class Snake : IPlaceable
    {
        public GameModel Game;

        public event EventHandler Removed;

        public Kaba Kaba { get; private set; }
        public Hiro Hiro { get; private set; }
        public Hana Hana { get; private set; }
        public Naru Naru { get; private set; }
        public Point Position => Kaba.Position;
        public int X => Position.X;
        public int Y => Position.Y;
        public IEnumerable<PlayerControlledEntity> Heroes
        {
            get
            {
                yield return Kaba;
                yield return Hiro;
                yield return Hana;
                yield return Naru;
            }
        }

        public PlayerScheduler Scheduler { get; protected set; } 

        public Snake(GameModel game, Point kabaPosition, Point hiroPosition, Point hanaPosition, Point naruPosition)
        {
            Kaba = new Kaba(game, kabaPosition);
            Hiro = new Hiro(game, hiroPosition);
            Hana = new Hana(game, hanaPosition);
            Naru = new Naru(game, naruPosition);
            Game = game;
            Scheduler = new PlayerScheduler(this);
        }

        public Snake(GameModel game, Kaba kaba, Hiro hiro, Hana hana, Naru naru)
        {
            Kaba = kaba;
            Hiro = hiro;
            Hana = hana;
            Naru = naru;
            Game = game;
            Scheduler = new PlayerScheduler(this);
        }

        public void MoveInDirection(Direction direction)
        {

            var previousPosition = Kaba.Position;
            Kaba.Move(direction);
            var oldHiroPosition = Hiro.Position;
            Hiro.MoveTo(previousPosition, true);
            var oldHanaPosition = Hana.Position;
            Hana.MoveTo(oldHiroPosition, true);
            Naru.MoveTo(oldHanaPosition, true);

        }

        public void PlaceItself(Level level)
        {

            level.PlaceObject(Kaba);
            level.PlaceObject(Hiro);
            level.PlaceObject(Hana);
            level.PlaceObject(Naru);

        }

        public void PlaceItself(Level level, Point positionForHead)
        {
            var hiroOffset = Kaba.Position - new Size(Hiro.Position);
            var hanaOffset = Hiro.Position - new Size(Hana.Position);
            var naruOffset = Hana.Position - new Size(Naru.Position);
            Kaba.Position = positionForHead;
            Hiro.Position = positionForHead + new Size(hiroOffset);
            Hana.Position = Hiro.Position + new Size(hanaOffset);
            Naru.Position = Hana.Position + new Size(naruOffset);

            PlaceItself(level);
        }

        public HashSet<Point> RecalculateVisionField(Level level)
        {
            var result = new HashSet<Point>();
            foreach (var hero in Heroes)
            {
                if (!hero.IsAlive)
                    continue;
                foreach (var point in hero.RecalculateVisionField(level))
                    result.Add(point);
            }
            return result;
        }

        public void Remove()
        {
            foreach (var hero in Heroes)
            {
                hero.Remove();
            }
        }
    }
}
