using System;
using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public partial class GameWindow : Form
    {
        private readonly Timer Timer;
        private readonly GameModel game;
        private readonly Random rnd;
        private readonly Entity Ent;
        private readonly int tileSize = 70;
        private readonly int whiteSpace = 10;
        private readonly float tileBorderWidth = 0.5f;
        private ScaledViewPanel panel;
        bool imageUpdateComplete = false;
        public GameWindow()
        {
            game = new GameModel(10, 10);
            Ent = new Kaba(5, 5);
            game.currentLevel.PlaceObject(Ent);
            rnd = new Random();
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

        void TimerTick(object sender, EventArgs e)
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
                        Ent.Move(Direction.Up);
                        break;
                    case Keys.A:
                        Ent.Move(Direction.Left);
                        break;
                    case Keys.S:
                        Ent.Move(Direction.Down);
                        break;
                    case Keys.D:
                        Ent.Move(Direction.Right);
                        break;
                }
                imageUpdateComplete = false;
            }
            
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    var g = e.Graphics;
        //    foreach (var tile in game.currentLevel)
        //    {
        //        DrawWithBorder(g, tile);
        //    }
        //}

        private (float x, float y) TileCoordinatesToWindow((int x, int y) coords)
        {
            var newX = whiteSpace + tileBorderWidth + coords.x * (tileSize + tileBorderWidth);
            var newY = whiteSpace + tileBorderWidth + coords.y * (tileSize + tileBorderWidth);
            return (newX, newY);
        }

        //private void DrawWithBorder(Graphics g, Tile tile)
        //{
        //    var coords = TileCoordinatesToWindow((tile.x, tile.y));
        //    g.DrawRectangle(Pens.Black, coords.x - tileBorderWidth, coords.y - tileBorderWidth, tileSize + tileBorderWidth, tileSize + tileBorderWidth);
        //    g.DrawImage(tile.GetSprite(), coords.x, coords.y, tileSize, tileSize);
        //    if (tile.GameObject != null)
        //        g.DrawImage(tile.GameObject.GetSprite(), coords.x + tileSize * 0.05f, coords.y + tileSize * 0.05f, tileSize * 0.9f, tileSize * 0.9f);
        //}
    }
}
