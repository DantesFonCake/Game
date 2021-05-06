using Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Game
{
    public partial class GameWindow : Form
    {
        private const int ButtonSize = 96;
        private readonly Timer Timer;
        private readonly GameModel Game;
        private readonly Controller Controller;
        private ScaledViewPanel mainGameView;
        private Dictionary<PlayerControlledEntity, CustomDrawableControl> customButtons;
        private CustomDrawableControl qActionButton;
        private CustomDrawableControl eActionButton;
        public event EventHandler<CustomMouseEventArg> ClickPerformed;

        public GameWindow()
        {
            Dock = DockStyle.Fill;
            Game = new GameModel(21, 21);
            Controller = new Controller(Game);
            Timer = new Timer();

            InitComponents();
        }

        private void InitComponents()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Width = 1280;
            Height = 720;
            MinimumSize = new Size(640, 480);
            DoubleBuffered = true;
            #region Button Creation
            var brush = new SolidBrush(Color.Aquamarine);
            #region Character Selection Button Creation
            customButtons = Game.Snake.Heroes.ToDictionary(x => x, x =>
            {

                var image = new Bitmap(Properties.Resources.icon_placeholder, new Size(ButtonSize, ButtonSize));
                using var g = Graphics.FromImage(image);
                g.DrawImage(x.Sprite, new Rectangle(new Point(ButtonSize / 2 - 25, 12), new Size(50, 50)));
                var b = new CustomDrawableControl(this)
                {
                    TextBrush = brush,
                    Image = image,
                    Size = new Size(ButtonSize, ButtonSize),
                    InactiveBorderWidth = 10,
                };
                b.AddText(new Point(ButtonSize / 2 + 7, 65), x.Name, new Font(Font.FontFamily, 15));
                return b;
            });
            var initialPositionX = 15;//ClientSize.Width / 2 - ButtonSize * customButtons.Count / 2;
            foreach (var (e, b) in customButtons)
            {
                b.Location = new Point(initialPositionX, ClientSize.Height - (ButtonSize + 5));
                initialPositionX += ButtonSize + 4;
                b.Lock();
                b.Click += (_, arg) =>
                {
                    Game.SelectEntity(e);
                };
            }
            #endregion
            #region Action Selection Button Creation
            qActionButton = new CustomDrawableControl(this)
            {
                TextBrush = brush,
                Image = Properties.Resources.icon_placeholder,
                Size = new Size(ButtonSize, ButtonSize),
                Location = new Point(5, ClientSize.Height / 2 - (ButtonSize / 2 + 1)),
                InactiveBorderWidth = 10,

            };
            qActionButton.AddText(new Point(ButtonSize - 25, 15), "Q", new Font(Font.FontFamily, 15));
            eActionButton = new CustomDrawableControl(this)
            {
                TextBrush = brush,
                Image = Properties.Resources.icon_placeholder,
                Size = new Size(ButtonSize, ButtonSize),
                Location = new Point(5, ClientSize.Height / 2 + (ButtonSize / 2 + 1)),
                InactiveBorderWidth = 10,
            };
            eActionButton.AddText(new Point(ButtonSize - 25, 15), "E", new Font(Font.FontFamily, 15));
            #endregion
            #endregion
            mainGameView = new ScaledViewPanel(this, Game, Controller);
            Timer.Interval = 1000 / 120;
            Timer.Tick += TimerTick;
            Timer.Start();
            var a = HasChildren;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (Game.SelectedEntity != null)
            {
                qActionButton.Enabled = true;
                eActionButton.Enabled = true;
                switch (Game.SelectedAttack)
                {
                    default:
                    case KeyedAttack.None:
                        qActionButton.Deselect();
                        eActionButton.Deselect();
                        break;
                    case KeyedAttack.EAttack:
                        eActionButton.Select();
                        qActionButton.Deselect();
                        break;
                    case KeyedAttack.QAttack:
                        eActionButton.Deselect();
                        qActionButton.Select();
                        break;
                }
            }
            else
            {
                qActionButton.Enabled = false;
                eActionButton.Enabled = false;
            }

            foreach (var (ent, b) in customButtons)
            {

                if (Game.SelectedEntity == ent)
                    b.Select();
                else
                    b.Deselect();
                b.Enabled = Game.IsAccessible;
            }
            Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(mainGameView.GetBitmap(), Point.Empty);
            qActionButton.Draw(e.Graphics);
            eActionButton.Draw(e.Graphics);
            foreach (var b in customButtons.Values)
            {
                b.Draw(e.Graphics);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //if (imageUpdateComplete)
            //{

            Controller.HandleKeyPress(e.KeyCode);
            //imageUpdateComplete = false;
            //}
        }

        protected override void OnMouseClick(MouseEventArgs e) => ClickPerformed?.Invoke(this, new CustomMouseEventArg(e));
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Text = e.Location.ToString();
        }

        private int GetPercentFromWidth(float percent) => (int)(percent != 0 ? ClientSize.Width * percent / 100 : 0);
        private int GetPercentFromHeight(float percent) => (int)(percent != 0 ? ClientSize.Height * percent / 100 : 0);
    }
}
