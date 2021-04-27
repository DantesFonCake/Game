using System;
using System.Windows.Forms;

namespace Game
{
    public partial class GameWindow : Form
    {
        private readonly Timer Timer;
        private readonly GameModel Game;
        private readonly Controller Controller;
        private readonly ScaledViewPanel panel;
        private bool imageUpdateComplete = false;
        public GameWindow()
        {
            Game = new GameModel(21, 21);
            Controller = new Controller(Game);
            panel = new ScaledViewPanel(Game,Controller) { Dock = DockStyle.Fill };
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
                Controller.HandleKeyPress(e.KeyCode);
                
                imageUpdateComplete = false;
            }

        }
    }
}
