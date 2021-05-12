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
        };
        private static readonly Dictionary<Type, GameObjectTypes> TypeToGameObjectType = new Dictionary<Type, GameObjectTypes>()
        {
            [typeof(Stone)] = GameObjectTypes.Stone,
            [typeof(Kaba)] = GameObjectTypes.Kaba,
            [typeof(Hana)] = GameObjectTypes.Hana,
            [typeof(Hiro)] = GameObjectTypes.Hiro,
            [typeof(Naru)] = GameObjectTypes.Naru,
            [typeof(BasicEnemy)] = GameObjectTypes.Enemy,
        };
        private static readonly Dictionary<GameObjectTypes, string> ObjectTypesRepr = TypesDictionary.ToFlippedDictionary();
        private static readonly Dictionary<GameObjectTypes, Type> GameObjectTypesToType = TypeToGameObjectType.ToFlippedDictionary();

        public static GameObjectTypes GetObjectType(this GameObject gameObject) => TypeToGameObjectType[gameObject.GetType()];

        public static string UToString(this GameObjectTypes type) => ObjectTypesRepr[type];

        public static GameObjectTypes StringToGameObject(this string s) => TypesDictionary[s];

        public static GameObject GenerateObject(this GameObjectTypes type, GameModel game, int x, int y) => (GameObject)GameObjectTypesToType[type].GetConstructor(new[] { typeof(GameModel), typeof(int), typeof(int) }).Invoke(new object[] { game, x, y });
        #endregion

        public static (Level Level, Snake Snake) FromString(GameModel game, string lines) 
            => FromLines(game, lines.Split('\n', '\r').Where(x => !string.IsNullOrEmpty(x)).ToArray());

        public static (Level, Snake) FromLines(GameModel game, string[] lines)
        {
            var tiles = lines.Select(line => line.Split(';').Select(tile => tile.Split(',')).ToArray()).ToArray();
            var xSize = tiles[0].Length;
            var ySize = tiles.Length;
            var level = new Level(game, xSize, ySize);
            Kaba kaba = null;
            Hiro hiro = null;
            Naru naru = null;
            Hana hana = null;
            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    foreach (var repr in tiles[y][x])
                    {
                        if (string.IsNullOrEmpty(repr)||string.IsNullOrWhiteSpace(repr))
                            continue;
                        var obj = repr.StringToGameObject().GenerateObject(game, x, y);
                        if (obj is Kaba)
                            kaba = (Kaba)obj;
                        else if (obj is Hiro)
                            hiro = (Hiro)obj;
                        else if (obj is Naru)
                            naru = (Naru)obj;
                        else if (obj is Hana)
                            hana = (Hana)obj;
                        level.PlaceObject(obj);
                    }
                }
            }
            if(kaba!=null&&hiro!=null&&naru!=null&&hana!=null)
                return (level, new Snake(game, kaba, hiro, hana, naru));
            return (level, null);
        }

        public static Level OfSize(GameModel game,Size size)
        {
            return new Level(game, size.Width, size.Height);
        }
    }
}
