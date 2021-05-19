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
        private CustomDrawableComponent hudContainer;
        private Dictionary<PlayerControlledEntity, CustomDrawableButton> customButtons;
        private CustomDrawableButton qActionButton;
        private CustomDrawableButton eActionButton;
        private readonly CustomDrawableButton collectItemButton;
        public event EventHandler<CustomMouseEventArg> ClickPerformed;

        public GameWindow()
        {
            Dock = DockStyle.Fill;
            Game = new GameModel(Levels.Level1);
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
            hudContainer = new CustomDrawableComponent(this)
            {
                Size = ClientSize,
                Location = Point.Empty,
            };
            #region Button Creation
            var hoveredColor = Color.FromArgb(96, Color.Green);
            var disabledColor = Color.FromArgb(96, Color.Gray);
            var selectedColor = Color.FromArgb(96, Color.Yellow);
            var textColor = Color.Aquamarine;
            #region Character Selection Button Creation
            customButtons = Game.Snake.Heroes.ToDictionary(x => x, x =>
            {

                var image = new Bitmap(Properties.Resources.icon_placeholder, new Size(ButtonSize, ButtonSize));
                using var g = Graphics.FromImage(image);
                g.DrawImage(x.Sprite, new Rectangle(new Point(ButtonSize / 2 - 25, 12), new Size(50, 50)));
                var b = new CustomDrawableButton(this)
                {
                    Image = image,
                    Size = new Size(ButtonSize, ButtonSize),
                    InactiveBorderWidth = 10,
                    HoveredColor = hoveredColor,
                    DisabledColor = disabledColor,
                    SelectedColor = selectedColor,
                };
                hudContainer.AddChild(b);
                var label = new CustomDrawableLabel(this)
                {
                    Location = new Point(ButtonSize / 2 - 25, ButtonSize - 27 - 10),
                    Size = new Size(b.ClipRectangle.Width - 5, b.ClipRectangle.Height - 2),
                    Text = x.Name,
                    Font = new Font(Font.FontFamily, 15),
                    TextColor = textColor,
                };
                b.AddChild(label);
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
                    if(e==Game.SelectedEntity)
                    {
                        Game.SelectEntity(null);
                        return;
                    }    
                    Game.SelectEntity(e);
                };
            }
            #endregion
            #region Action Selection Button Creation
            qActionButton = new CustomDrawableButton(this)
            {
                Image = Properties.Resources.icon_placeholder,
                Size = new Size(ButtonSize, ButtonSize),
                Location = new Point(5, ClientSize.Height / 2 - (ButtonSize / 2 + 1)),
                InactiveBorderWidth = 10,
                HoveredColor = hoveredColor,
                DisabledColor = disabledColor,
                SelectedColor = selectedColor,

            };
            var label = new CustomDrawableLabel(this)
            {
                //Location = new Point(ButtonSize - 25, 15),
                Location = new Point(30, 5),
                Text = "Q",
                TextColor = textColor,
                Font = new Font(Font.FontFamily, 12),
                Size = new Size(qActionButton.ClipRectangle.Width - 5, qActionButton.ClipRectangle.Height - 2),
            };
            qActionButton.AddChild(label);
            qActionButton.Click += (s, e) =>
            {
                if (Game.SelectedAttack == KeyedAttack.QAttack)
                {
                    Game.SelectAttack(KeyedAttack.None);
                    return;
                }
                Game.SelectAttack(KeyedAttack.QAttack);
            };
            hudContainer.AddChild(qActionButton);
            eActionButton = new CustomDrawableButton(this)
            {
                Image = Properties.Resources.icon_placeholder,
                Size = new Size(ButtonSize, ButtonSize),
                Location = new Point(5, ClientSize.Height / 2 + ButtonSize / 2 + 1),
                InactiveBorderWidth = 10,
                HoveredColor = hoveredColor,
                DisabledColor = disabledColor,
                SelectedColor = selectedColor,
            };
            label = new CustomDrawableLabel(this)
            {
                //Location = new Point(ButtonSize-25, 15),
                Location = new Point(30, 5),
                Text = "E",
                TextColor = textColor,
                Font = new Font(Font.FontFamily, 12),
                Size = new Size(qActionButton.ClipRectangle.Width - 5, qActionButton.ClipRectangle.Height - 2),
            };
            eActionButton.AddChild(label);
            hudContainer.AddChild(eActionButton);
            eActionButton.Click += (s, e) =>
            {
                if (Game.SelectedAttack == KeyedAttack.EAttack)
                {
                    Game.SelectAttack(KeyedAttack.None);
                    return;
                }
                Game.SelectAttack(KeyedAttack.EAttack);
            };
            #endregion
            #region Item Collect Button Creation
            //collectItemButton = new CustomDrawableComponent(this)
            //{
            //    TextBrush = textColor,
            //    Image = Properties.Resources.icon_placeholder,
            //    Size = new Size(ButtonSize, ButtonSize),
            //    Location = new Point(ClientSize.Width-ButtonSize-5, ClientSize.Height / 2 - (ButtonSize / 2)),
            //    InactiveBorderWidth = 10,
            //};
            #endregion
            #endregion
            mainGameView = new ScaledViewPanel(this, Game, Controller);
            Timer.Interval = 1000 / 120;
            Timer.Tick += TimerTick;
            Timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Text = $"LogicalLocation: {mainGameView.MouseLogicalPos}, StepActionsLeft: {Game.PlayerScheduler.MaxActionCount-Game.PlayerScheduler.ActionCount}";

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
                b.Enabled = Game.IsAccessible && ent.IsAlive;
            }
            Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(mainGameView.GetBitmap(), Point.Empty);
            hudContainer.Draw(e.Graphics);
            //qActionButton.Draw(e.Graphics);
            //eActionButton.Draw(e.Graphics);
            //foreach (var b in customButtons.Values)
            //{
            //    b.Draw(e.Graphics);
            //}
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

        private int GetPercentFromWidth(float percent) => (int)(percent != 0 ? ClientSize.Width * percent / 100 : 0);
        private int GetPercentFromHeight(float percent) => (int)(percent != 0 ? ClientSize.Height * percent / 100 : 0);
    }
}
