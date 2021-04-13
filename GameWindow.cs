using System;
using System.Windows.Forms;

namespace Game
{
    public partial class GameWindow : Form
    {
        private readonly Timer Timer;
        private readonly GameModel game;
        private readonly Kaba kaba;
        private readonly ScaledViewPanel panel;
        private bool imageUpdateComplete = false;
        public GameWindow()
        {
            kaba = new Kaba(5, 5);
            game = new GameModel(21, 21, true, kaba);
            panel = new ScaledViewPanel(game) { Dock = DockStyle.Fill };
            InitializeComponent();
            Controls.Add(panel);
            DoubleBuffered = true;
            Width = 1100;
            Height = 1100;
            Timer = new Timer
            {
                Interval = 1000 / 60
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
                        kaba.Move(Direction.Up);
                        break;
                    case Keys.A:
                        kaba.Move(Direction.Left);
                        break;
                    case Keys.S:
                        kaba.Move(Direction.Down);
                        break;
                    case Keys.D:
                        kaba.Move(Direction.Right);
                        break;
                }
                imageUpdateComplete = false;
            }

        }
    }
}
