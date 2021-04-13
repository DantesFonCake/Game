using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game
{
    public class LevelCreationTests
    {
        [Test]
        public void Test_HaveLevel()
        {
            var game = new GameModel(10, 10, false);
            Assert.IsTrue(HaveLevel(game));
        }

        private static bool HaveLevel(GameModel game) => game.currentLevel != null && game.currentLevel.Map != null && game.currentLevel.Map.All(x => x != null);



        [Test]
        public void Test_LevelsHaveNormalSizes([Random(1, 100, 10)] int x, [Random(1, 100, 10)] int y)
        {
            var game = new GameModel(x, y, false);
            game.MoveToNextLevel(x, y);
            Assert.IsTrue(HaveLevel(game));
            Assert.IsTrue(game.currentLevel.XSize == x && game.currentLevel.YSize == y);
        }
    }
    public class EntityBasicTests
    {
        [TestCase(1, 2, 10, 10)]
        [TestCase(1, 2, 5, 3)]
        [TestCase(1, 2, 2, 3)]
        [TestCase(1, 2, 3, 10)]
        public void Test_EntityExist(int x, int y, int sizeX, int sizeY)
        {
            var entity = new Kaba(x, y);
            var game = new GameModel(sizeX, sizeY, false, entity);
            game.currentLevel.PlaceObject(entity);
            Assert.IsTrue(game.currentLevel[x, y].GameObjects.First() is Entity && ((Entity)game.currentLevel[x, y].GameObjects.First()) == entity);
        }

        [Test]
        public void Test_EntityCanMove([Random(4, Distinct = true)] Direction direction)
        {
            var entity = new Kaba(5, 5);
            var game = new GameModel(10, 10, false, entity);
            entity.Move(direction);
            var (dX, dY) = Utils.GetOffsetFromDirection(direction);
            Assert.IsTrue(CheckForEntity(5 + dX, 5 + dY, entity, game));
        }

        [Test]
        public void Test_EntityCantMoveOutOfBounds()
        {
            var entity = new Kaba(9, 9);
            var game = new GameModel(10, 10, false, entity);
            entity.Move(Direction.Right);
            Assert.IsTrue(CheckForEntity(9, 9, entity, game));
        }

        private static bool CheckForEntity(int x, int y, Entity entity, GameModel game) => entity.X == x && entity.Y == y && game.currentLevel[x, y].GameObjects.First() is Entity && (Entity)game.currentLevel[x, y].GameObjects.First() == entity;

        [Test]
        public void Test_EntityCantGoOverRigidObject()
        {
            var entity = new Kaba(5, 5);
            var stone = new Stone(5, 4);
            var game = new GameModel(10, 10, false, entity, stone);
            entity.Move(Direction.Up);
            Assert.IsTrue(CheckForEntity(5, 5, entity, game));
        }
    }
}
