using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Game.Model
{
    public static class LevelCreator
    {
        #region String-GameObjectTypes-Object Connection
        private static readonly Dictionary<string, GameObjectTypes> TypesDictionary = new Dictionary<string, GameObjectTypes>()
        {
            ["S"] = GameObjectTypes.Stone,
            ["Ha"] = GameObjectTypes.Hana,
            ["Ka"] = GameObjectTypes.Kaba,
            ["Hi"] = GameObjectTypes.Hiro,
            ["Na"] = GameObjectTypes.Naru,
            ["E"] = GameObjectTypes.Enemy,
            ["CI"] = GameObjectTypes.CollectableItem,
            ["Ex"] = GameObjectTypes.Exit,
        };
        private static readonly Dictionary<Type, GameObjectTypes> TypeToGameObjectType = new Dictionary<Type, GameObjectTypes>()
        {
            [typeof(Stone)] = GameObjectTypes.Stone,
            [typeof(Kaba)] = GameObjectTypes.Kaba,
            [typeof(Hana)] = GameObjectTypes.Hana,
            [typeof(Hiro)] = GameObjectTypes.Hiro,
            [typeof(Naru)] = GameObjectTypes.Naru,
            [typeof(BasicEnemy)] = GameObjectTypes.Enemy,
            [typeof(CollectableGameObject)] = GameObjectTypes.CollectableItem,
            [typeof(Exit)] = GameObjectTypes.Exit,
        };
        private static readonly Dictionary<GameObjectTypes, string> ObjectTypesRepr = TypesDictionary.ToFlippedDictionary();
        private static readonly Dictionary<GameObjectTypes, Type> GameObjectTypesToType = TypeToGameObjectType.ToFlippedDictionary();

        public static GameObjectTypes GetObjectType(this GameObject gameObject) => TypeToGameObjectType[gameObject.GetType()];

        public static string UToString(this GameObjectTypes type) => ObjectTypesRepr[type];

        public static GameObjectTypes StringToGameObject(this string s) => TypesDictionary[s];

        public static GameObject GenerateObject(this GameObjectTypes type, GameModel game, int x, int y)
        {
            if (type == GameObjectTypes.CollectableItem)
            {
                var random = Utils.GetRandomInt();
                var itemType = random > 50 ? random > 80 ? ItemTypes.Shield : ItemTypes.SharpeningStone : ItemTypes.Boots;
                return CollectableItemsFactory.Items[itemType](game, new Point(x, y));
            }
            return (GameObject)GameObjectTypesToType[type].GetConstructor(new[] { typeof(GameModel), typeof(int), typeof(int) }).Invoke(new object[] { game, x, y });
        }
        #endregion

        public static Level FromString(GameModel game, string lines, out Snake snake, out List<BasicEnemy> enemies)
            => FromLines(game, lines.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)).ToArray(), out enemies, out snake);

        public static Level FromLines(GameModel game, string[] lines, out List<BasicEnemy> enemies, out Tuple<Point, Point, Point, Point> nextSnakeposition)
        {
            var tiles = lines.Select(line => line.Split(';').Select(tile => tile.Split(',')).ToArray()).ToArray();
            var xSize = tiles[0].Length;
            var ySize = tiles.Length;
            var level = new Level(game, xSize, ySize);
            enemies = new List<BasicEnemy>();
            Point? kabaPosition = null, hiroPosition = null, naruPosition = null, hanaPosition = null;
            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    foreach (var repr in tiles[y][x])
                    {
                        if (string.IsNullOrEmpty(repr) || string.IsNullOrWhiteSpace(repr))
                            continue;
                        var type = repr.StringToGameObject();
                        switch (type)
                        {
                            case GameObjectTypes.Hana:
                                hanaPosition = new Point(x, y);
                                break;
                            case GameObjectTypes.Kaba:
                                kabaPosition = new Point(x, y);
                                break;
                            case GameObjectTypes.Hiro:
                                hiroPosition = new Point(x, y);
                                break;
                            case GameObjectTypes.Naru:
                                naruPosition = new Point(x, y);
                                break;
                            default:
                                var obj = repr.StringToGameObject().GenerateObject(game, x, y);
                                if (obj is BasicEnemy enemy)
                                    enemies.Add(enemy);
                                level.PlaceObject(obj);
                                break;
                        }
                    }
                }
            }
            nextSnakeposition = kabaPosition.HasValue && hiroPosition.HasValue && hanaPosition.HasValue && naruPosition.HasValue
                ? Tuple.Create(kabaPosition.Value, hiroPosition.Value, hanaPosition.Value, naruPosition.Value)
                : null;
            return level;
        }

        public static Level FromLines(GameModel game, string[] lines, out List<BasicEnemy> enemies, out Snake snake)
        {
            var level = FromLines(game, lines, out enemies, out Tuple<Point, Point, Point, Point> snakePosition);
            snake = snakePosition != null ? new Snake(game, snakePosition) : null;
            return level;
        }

        public static Level OfSize(GameModel game, Size size) => new Level(game, size.Width, size.Height);
    }
}
