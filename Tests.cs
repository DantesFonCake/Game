using NUnit.Framework;
using System;
using System.Drawing;
using System.Linq;

namespace Game
{
    public class LevelCreationTests
    {
        [Test]
        public void Test_HaveLevel()
        {
            var game = new GameModel(10, 10, false,false);
            Assert.IsTrue(HaveLevel(game));
        }

        private static bool HaveLevel(GameModel game) => game.CurrentLevel != null && game.CurrentLevel.Map != null && game.CurrentLevel.Map.All(x => x != null);



        [Test]
        public void Test_LevelsHaveNormalSizes([Random(1, 100, 10)] int x, [Random(1, 100, 10)] int y)
        {
            var game = new GameModel(x, y, false,false);
            game.MoveToNextLevel(x, y);
            Assert.IsTrue(HaveLevel(game));
            Assert.IsTrue(game.CurrentLevel.XSize == x && game.CurrentLevel.YSize == y);
        }
    }

    public class EntityBasicTests
    {
        [Test]
        [TestCase(1, 2, 10, 10)]
        [TestCase(1, 2, 5, 3)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(1, 2, 3, 10)]
        public void Test_EntityExist(int x, int y, int sizeX, int sizeY)
        {
            var entity = new Kaba(x, y);
            var game = new GameModel(sizeX, sizeY, false,false, entity);
            game.CurrentLevel.PlaceObject(entity);
            Assert.IsTrue(game.CurrentLevel[x, y].GameObjects.First() is Entity entity1 && entity1 == entity);
        }

        [Test]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        public void Test_EntityCanMove(Direction direction)
        {
            var entity = new Kaba(5, 5);
            var game = new GameModel(10, 10, false,false, entity);
            var initialPosition = entity.Position;
            entity.Move(direction);
            var offset = Utils.GetOffsetFromDirection(direction);
            Assert.IsTrue(CheckForEntity(initialPosition + offset, entity, game));
        }

        [Test]
        public void Test_EntityCantMoveOutOfBounds()
        {
            var entity = new Kaba(9, 9);
            var game = new GameModel(10, 10, false, false,entity);
            entity.Move(Direction.Right);
            Assert.IsTrue(CheckForEntity(new Point(9, 9), entity, game));
        }

        private static bool CheckForEntity(Point position, Entity entity, GameModel game) => entity.Position == position && game.CurrentLevel[position].GameObjects.First() is Entity entity1 && entity1 == entity;

        [Test]
        public void Test_EntityCantGoOverRigidObject()
        {
            var entity = new Kaba(5, 5);
            var stone = new Stone(5, 4);
            var game = new GameModel(10, 10, false, false,entity, stone);
            entity.Move(Direction.Up);
            Assert.IsTrue(CheckForEntity(new Point(5, 5), entity, game));
        }
    }

    public class AttackTests
    {
        [Test]
        public void Test_AttackDealsDamage()
        {
            var entities = new[] { new Kaba(5, 5), new Kaba(5, 6) };
            var game = new GameModel(10, 10, false,false, entities);
            entities[0].AttackPosition(entities[0].Position);
            Assert.IsTrue(entities[1].Health != 100);
        }

        [Test]
        public void Test_EntityDies()
        {

        }
    }

    public class SteppingTests
    {
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        [TestCase(Direction.Up)]
        public void Test_StepWorks(Direction direction)
        {
            var game = new GameModel(10, 10);
            var initialPosition = game.Snake.Position;
            game.TrySheduleMove(direction);
            game.CommitStep();
            while (!game.IsAccessible)
                continue;
            Assert.IsTrue(Utils.GetDirectionFromOffset(game.Snake.Position - new Size(initialPosition)) == direction);
        }

        [Test]
        public void Test_CantScheduleToUnrecheableTile()
        {
            var stone = new Stone(5, 4);
            var game = new GameModel(10, 10, false,true, stone);
            game.TrySheduleMove(Direction.Up);
            Assert.IsFalse(game.Step.CommitStep());
            game = new GameModel(10, 10, false,true);
            while (game.Snake.Position.Y > 0)
                game.Snake.Move(Direction.Up);
            game.TrySheduleMove(Direction.Up);
            Assert.IsFalse(game.Step.CommitStep());
        }
    }
}
