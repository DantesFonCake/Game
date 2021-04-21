using System;
using System.Windows.Forms;

namespace Game
{
    public partial class GameWindow : Form
    {
        private readonly Timer Timer;
        private readonly GameModel Game;
        private readonly ScaledViewPanel panel;
        private bool imageUpdateComplete = false;
        public GameWindow()
        {
            Game = new GameModel(21, 21);
            panel = new ScaledViewPanel(Game) { Dock = DockStyle.Fill };
            Controls.Add(panel);
            DoubleBuffered = true;
            Width = 1100;
            Height = 1100;
            Timer = new Timer
            {
                Interval = 1000 / 120
            };
            Timer.Tick += TimerTick;
            Load += (s, e) => Timer.Start();
            KeyDown += GameWindow_KeyDown;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            panel.Invalidate();
            Invalidate();
            imageUpdateComplete = true;
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (imageUpdateComplete)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        //Game.MoveHeroesInDirection(Direction.Up);
                        Game.SheduleMove(Direction.Up);
                        break;
                    case Keys.A:
                        //Game.MoveHeroesInDirection(Direction.Left);
                        Game.SheduleMove(Direction.Left);
                        break;
                    case Keys.S:
                        //Game.MoveHeroesInDirection(Direction.Down);
                        Game.SheduleMove(Direction.Down);
                        break;
                    case Keys.D:
                        //Game.MoveHeroesInDirection(Direction.Right);
                        Game.SheduleMove(Direction.Right);
                        break;
                    case Keys.Space:
                        Game.CommitStep();
                        break;
                    case Keys.E:
                        Game.PrepareForAttack();
                        break;
                }
                imageUpdateComplete = false;
            }

        }
    }
}
