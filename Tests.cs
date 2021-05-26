using Game.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Game
{
    internal class TestEnemy : BasicEnemy
    {
        public TestEnemy(GameModel game, Point position) : base(game, position)
        {
            Attack = new Attack(new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1) }, AttackType.Physical, 50, 2, false);
            movePossibilities = new[] { new Size(1, 0), new Size(0, 1), new Size(-1, 0), new Size(0, -1), new Size(0, 0) };
        }

        public TestEnemy(GameModel game, int x, int y) : this(game, new Point(x, y))
        {
        }
    }
    internal class TestEntity : Entity
    {
        public override BasicDrawer Drawer { get; protected set; } = null;
        public override Bitmap Sprite => null;
        public Scheduler Scheduler;

        public TestEntity(int x, int y) : base(null, x, y) => Scheduler = new Scheduler();

        public void SetAttack(Attack attack) => Attack = attack;
        public void SetResistance(AttackType type, int resistance) => resistances[type] = resistance;
    }

    internal class TestObject : GameObject
    {
        public override BasicDrawer Drawer { get; protected set; } = null;
        public override Bitmap Sprite => null;

        public TestObject(int x, int y) : base(null, x, y) => IsRigid = true;
    }

    public class LevelCreationTests
    {
        private static readonly string[] TestLevel1 = new[]
        {
            " ; ; ",
            " ; ; ",
            " ; ; "
        };
        [Test]
        public void Test_HaveLevel()
        {
            var game = new GameModel(TestLevel1);
            Assert.IsTrue(HaveLevel(game));
        }

        private static bool HaveLevel(GameModel game) => game.CurrentLevel != null && game.CurrentLevel.Map != null && game.CurrentLevel.Map.All(x => x != null);



        [Test]
        public void Test_LevelsHaveNormalSizes([Random(1, 20, 5)] int x, [Random(1, 20, 5)] int y)
        {
            var line = new StringBuilder();
            for (var i = 0; i < x; i++)
            {
                line.Append(" ;");
            }
            line.Remove(line.Length - 1, 1);
            var level = Enumerable.Range(0, y).Select(x => line.ToString()).ToArray();
            var game = new GameModel(level);
            Assert.IsTrue(HaveLevel(game));
            Assert.IsTrue(game.CurrentLevel.XSize == x && game.CurrentLevel.YSize == y);
            line = line.Clear();
            for (var i = 0; i < x + 1; i++)
            {
                line.Append(" ;");
            }
            line.Remove(line.Length - 1, 1);
            level = Enumerable.Range(0, y + 1).Select(x => line.ToString()).ToArray();
            game.MoveToNextLevel(level);
            Assert.IsTrue(HaveLevel(game));
            Assert.IsTrue(game.CurrentLevel.XSize == x + 1 && game.CurrentLevel.YSize == y + 1);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        [TestCase(2, 0)]
        [TestCase(2, 1)]
        [TestCase(2, 2)]
        public void Test_LevelCorrectObjectPlacement(int x, int y)
        {
            var level = (string[])TestLevel1.Clone();
            var line = new StringBuilder();
            for (var i = 0; i < level[0].Split(';').Length; i++)
            {
                if (i == x)
                    line.Append("S;");
                else
                    line.Append(" ;");
            }
            line.Remove(line.Length - 1, 1);
            level[y] = line.ToString();
            var l = LevelCreator.FromLines(null, level, out var enemies, out Snake snake);
            Assert.IsTrue(l.InBounds(x, y) && l[x, y].GameObjects.Length > 0);
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
            var level = LevelCreator.OfSize(null, new Size(sizeX, sizeY));
            var entity = new TestEntity(x, y);
            level.PlaceObject(entity);
            Assert.IsTrue(level.InBounds(x, y) && level[x, y].GameObjects.Contains(entity));
        }

        [Test]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        public void Test_EntityCanMove(Direction direction)
        {
            var level = LevelCreator.OfSize(null, new Size(10, 10));
            var entity = new TestEntity(5, 5);
            level.PlaceObject(entity);
            Assert.IsTrue(level.InBounds(5, 5) && level[5, 5].GameObjects.Contains(entity));
            entity.Move(direction);
            var position = new Point(5, 5) + direction.GetOffsetFromDirection();
            Assert.IsTrue(CheckForEntity(position, entity, level));
        }

        [Test]
        [TestCase(Direction.Up)]
        [TestCase(Direction.Down)]
        [TestCase(Direction.Left)]
        [TestCase(Direction.Right)]
        public void Test_EntityCantMoveOutOfBounds(Direction direction)
        {
            var level = LevelCreator.OfSize(null, new Size(1, 1));
            var entity = new TestEntity(0, 0);
            level.PlaceObject(entity);
            entity.Move(direction);
            Assert.IsTrue(CheckForEntity(new Point(0, 0), entity, level));

        }

        private static bool CheckForEntity(Point position, Entity entity, Level level) => entity.Position == position && level[position].GameObjects.Contains(entity);

        [Test]
        public void Test_EntityCantGoOverRigidObject()
        {
            var level = LevelCreator.OfSize(null, new Size(1, 2));
            var entity = new TestEntity(0, 0);
            var obj = new TestObject(0, 1);
            level.PlaceObject(entity);
            level.PlaceObject(obj);
            entity.Move(Direction.Down);
            Assert.IsTrue(CheckForEntity(new Point(0, 0), entity, level));
        }
    }

    public class AttackTests
    {
        private TestEntity Entity;
        private TestEntity Entity1;
        private Level Level;
        [SetUp]
        public void CreateAll()
        {
            Level = LevelCreator.OfSize(null, new Size(2, 1));
            Entity = new TestEntity(0, 0);
            Entity1 = new TestEntity(1, 0);
            Level.PlaceObject(Entity);
            Level.PlaceObject(Entity1);
            Entity.Rotate(Direction.Right);
        }

        [Test]
        public void Test_AttackDealsDamage()
        {
            Entity.SetAttack(new Attack(new[] { new Size(1, 0) }, AttackType.Fire, 10, 1, false));
            var initialHealth = Entity1.Health;
            Entity.AttackPosition(Entity.Position);
            Assert.IsTrue(Entity1.Health < initialHealth);
        }

        [Test]
        public void Test_EntityDies()
        {
            Entity.SetAttack(new Attack(new[] { new Size(1, 0) }, AttackType.Fire, (int)(Entity1.Health + 1), 1, false));
            Entity.AttackPosition(Entity.Position);
            Assert.IsTrue(!Entity1.IsAlive);
        }

        [Test]
        public void Test_AttackTypeResistanceReducesDamage([Random(10, 100, 10)] int resistance)
        {
            var initialDamage = 20;
            var initialHealth = Entity1.Health;
            Entity.SetAttack(new Attack(new[] { new Size(1, 0) }, AttackType.Physical, initialDamage, 1, false));
            Entity1.SetResistance(AttackType.Physical, resistance);
            Entity.AttackPosition(Entity.Position);
            var dealtDamage = initialHealth - Entity1.Health;
            Assert.AreEqual(100 - dealtDamage / initialDamage * 100, resistance, 1e-3);
        }
    }

    public class SteppingTests
    {
        private TestEntity Entity;
        private Level Level;

        [SetUp]
        public void CreateAll()
        {
            Entity = new TestEntity(0, 0);
            Entity.SetAttack(new Attack(new[] { new Size(1, 0) }, AttackType.Fire, 10, 1, true));
            Level = LevelCreator.OfSize(null, new Size(3, 3));
            Level.PlaceObject(Entity);
        }

        [TestCase(new[] { Direction.Right })]
        [TestCase(new[] { Direction.Right, Direction.Down, Direction.Up, Direction.Left })]
        [TestCase(new[] { Direction.Right, Direction.Right, Direction.Down })]
        public void Test_MovementSteps(Direction[] path)
        {
            var position = Entity.Position;
            foreach (var dir in path)
            {
                Entity.Scheduler.AddMovement(x => Entity.Move(x), dir);
                position += dir.GetOffsetFromDirection();
            }
            while (Entity.Scheduler.Commit()) ;
            Assert.AreEqual(position, Entity.Position);
        }

        [TestCase(new[] { Direction.Right })]
        [TestCase(new[] { Direction.Right, Direction.Down, Direction.Up, Direction.Left })]
        [TestCase(new[] { Direction.Right, Direction.Right, Direction.Down })]
        public void Test_StepsUnchedule(Direction[] path)
        {
            var position = Entity.Position;
            var count = 0;
            foreach (var dir in path)
            {
                Entity.Scheduler.AddMovement(x => Entity.Move(x), dir);
                count++;
            }
            for (var i = 0; i < count; i++)
            {
                Entity.Scheduler.Unschedule();
            }
            while (Entity.Scheduler.Commit()) ;
            Assert.AreEqual(position, Entity.Position);
        }

        [Test]
        public void Test_AttackSteps()
        {
            var entity1 = new TestEntity(0, 1);
            Level.PlaceObject(entity1);
            var initialHealth = entity1.Health;
            Entity.Scheduler.AddAttack(Entity, Entity.Position);
            Entity.Scheduler.Commit();
            Assert.IsTrue(entity1.Health < initialHealth);
        }

        [Test]
        public void Test_MixedSteps()
        {
            var entity1 = new TestEntity(2, 0);
            var initialHealth = entity1.Health;
            Level.PlaceObject(entity1);
            Entity.Scheduler.AddMovement(x => Entity.Move(x), Direction.Right);
            Entity.Scheduler.AddAttack(Entity, Entity.Position + Direction.Right.GetOffsetFromDirection(), Direction.Right);
            Entity.Scheduler.AddMovement(x => Entity.Move(x), Direction.Down);
            Entity.Scheduler.AddMovement(x => Entity.Move(x), Direction.Right);
            Entity.Scheduler.Commit();
            Assert.IsTrue(new Point(1, 0) == Entity.Position && entity1.Health == initialHealth);
            Entity.Scheduler.Commit();
            Assert.IsTrue(new Point(1, 0) == Entity.Position && entity1.Health < initialHealth);
            initialHealth = entity1.Health;
            while (Entity.Scheduler.Commit()) ;
            Assert.IsTrue(new Point(2, 1) == Entity.Position && entity1.Health == initialHealth);
        }
    }

    public class AITests
    {
        private GameModel game;
        private Level level;
        private BasicAI ai;
        private TestEnemy testEnemy;

        [SetUp]
        public void SetUp()
        {
            game = new GameModel(levelLines);
            level = game.CurrentLevel;
            testEnemy = new TestEnemy(game, new Point(1,1));
            level.PlaceObject(testEnemy);
            ai = new BasicAI(game, new List<BasicEnemy> { testEnemy });
        }

        private static readonly string[] levelLines = new[]
        {
            " ; ; ; ; ",
            " ; ; ; ;",
            " ; ; ; ; "
        };

        [Test]
        public void Test_CorrectlyMovesToPosition()
        {
            var targets = new[] { new Point(2, 2) };
            for (var i = 0; i < 2; i++)
            {
                ai.ScheduleTowards(level, testEnemy, targets);
                testEnemy.Scheduler.Commit();
            }
            Assert.AreEqual(targets[0], testEnemy.Position);
        }

        [Test]
        public void Test_CorrectlyAttacks()
        {
            var enemy1 = new TestEntity(1, 0);
            var initialHealth = enemy1.Health;
            level.PlaceObject(enemy1);
            ai.TryScheduleAttack(level, testEnemy, new[] { enemy1.Position });
            while (testEnemy.Scheduler.Commit()) ;
            Assert.IsTrue(enemy1.Health < initialHealth);
        }

        [Test]
        public void Test_CorrectlyDoesMixedActions()
        {
            var initialPosition = testEnemy.Position;
            var enemy1 = new TestEntity(4, 2);
            var initialHealth = enemy1.Health;
            level.PlaceObject(enemy1);
            var enemyPosition = new[] { enemy1.Position };
            while (!ai.TryScheduleAttack(level, testEnemy, enemyPosition))
            {
                ai.ScheduleTowards(level, testEnemy, enemyPosition);
                while (testEnemy.Scheduler.Commit()) ;
            }
            while (testEnemy.Scheduler.Commit()) ;
            Assert.AreNotEqual(initialPosition, testEnemy.Position);
            Assert.IsTrue(enemy1.Health < initialHealth);
        }
    }
}
