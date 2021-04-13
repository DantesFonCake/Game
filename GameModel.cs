using System.Collections.Generic;

namespace Game
{
    public class GameModel
    {
        public Level currentLevel;
        public List<Direction> SheduledPath = new List<Direction>();

        public GameModel(int x, int y, bool generateWalls, params GameObject[] objects)
        {
            currentLevel = new Level(x, y);
            foreach (var obj in objects)
            {
                currentLevel.PlaceObject(obj);
            }
            if (generateWalls)
                for (var yCoord = 0; yCoord < currentLevel.YSize; yCoord++)
                    for (var xCoord = 0; xCoord < currentLevel.XSize; xCoord++)
                        if (xCoord == 0 || xCoord == currentLevel.XSize - 1 || yCoord == 0 || yCoord == currentLevel.YSize - 1)
                            currentLevel.PlaceObject(new Stone(xCoord, yCoord));
        }

        //TODO
        public void MoveToNextLevel(int x, int y) => currentLevel = new Level(x, y);
    }
}
