using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    public class GameModel
    {
        public Level currentLevel;

        public GameModel(int x=10,int y=10)
        {
            currentLevel = new Level(x, y);
        }

        public void MoveToNextLevel(int x,int y)
        {
            currentLevel = new Level(x, y);
        }

        public void OnMouseDown(Point point) 
        {
            currentLevel.PlaceObject(new Kaba(point.X, point.Y));
        }
    }
}
