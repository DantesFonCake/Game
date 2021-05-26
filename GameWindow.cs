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
        private readonly Font MainFont = new Font(FontFamily.GenericSerif, 25, GraphicsUnit.Pixel);
        private const int ButtonSize = 96;
        private readonly Timer Timer;
        private new Point MousePosition;
        private readonly GameModel Game;
        private readonly Controller Controller;
        private ScaledViewPanel mainGameView;
        private CustomDrawableComponent hudContainer;
        private FloatingTooltip tooltip;
        private Dictionary<PlayerControlledEntity, CustomDrawableButton> customButtons;
        private readonly Dictionary<KeyedAttack, CustomDrawableButton> actionButtons = new Dictionary<KeyedAttack, CustomDrawableButton>();
        private readonly Dictionary<KeyedAttack, ProgressBar> actionBars = new Dictionary<KeyedAttack, ProgressBar>();
        public event EventHandler<CustomMouseEventArg> ClickPerformed;

        public GameWindow()
        {
            Dock = DockStyle.Fill;
            Game = new GameModel();
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
            tooltip = new FloatingTooltip(this, 25)
            {
                Size = new Size(96*3, 96*2),
                Visible = false,
                Font = MainFont,
                BackColor = ColorTranslator.FromHtml("#094f0f"),
            };
            hudContainer.AddChild(tooltip);
            #region Button Creation
            var hoveredColor = Color.FromArgb(96, Color.Green);
            var disabledColor = Color.FromArgb(96, Color.Gray);
            var selectedColor = Color.FromArgb(96, Color.Yellow);
            var textColor = Color.Black;
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
                    Font = MainFont,
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
                    if (e == Game.SelectedEntity)
                    {
                        Game.SelectEntity(null);
                        return;
                    }
                    Game.SelectEntity(e);
                };
            }
            #endregion
            #region Action Selection Button Creation
            var barColor = Color.FromArgb(40, Color.Red);
            var qActionButton = new CustomDrawableButton(this)
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
                Location = new Point(ButtonSize - 35, 15),
                Text = "Q",
                TextColor = textColor,
                Font = MainFont,
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
            actionButtons[KeyedAttack.QAttack] = qActionButton;
            var qActionBar = new ProgressBar(this,
                0, 100,
                () => (int)(Game.SelectedEntity != null
                    ? (double)Game.SelectedEntity.Cooldowns.GetValueOrDefault(KeyedAttack.QAttack, 0)
                        / Game.SelectedEntity.QAttack.Cooldown * 100 : 0))
            {
                Location = Point.Empty,
                Size = qActionButton.ClipRectangle.Size,
                InactiveBorderWidth = 0,
                CompletedColor = barColor,
            };
            actionBars[KeyedAttack.QAttack] = qActionBar;
            qActionButton.AddChild(qActionBar);
            hudContainer.AddChild(qActionButton);
            var eActionButton = new CustomDrawableButton(this)
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
                Location = new Point(ButtonSize - 35, 15),
                Text = "E",
                TextColor = textColor,
                Font = MainFont,
                Size = new Size(qActionButton.ClipRectangle.Width - 5, qActionButton.ClipRectangle.Height - 2),
            };
            eActionButton.AddChild(label);
            eActionButton.Click += (s, e) =>
            {
                if (Game.SelectedAttack == KeyedAttack.EAttack)
                {
                    Game.SelectAttack(KeyedAttack.None);
                    return;
                }
                Game.SelectAttack(KeyedAttack.EAttack);
            };
            actionButtons[KeyedAttack.EAttack] = eActionButton;
            var eActionBar = new ProgressBar(this,
                0, 100,
                () => (int)(Game.SelectedEntity != null
                    ? (double)Game.SelectedEntity.Cooldowns.GetValueOrDefault(KeyedAttack.EAttack, 0)
                        / Game.SelectedEntity.EAttack.Cooldown * 100
                    : 0))
            {
                Location = Point.Empty,
                Size = qActionButton.ClipRectangle.Size,
                InactiveBorderWidth = 0,
                CompletedColor = barColor,
            };
            eActionButton.AddChild(eActionBar);
            actionBars[KeyedAttack.EAttack] = eActionBar;
            hudContainer.AddChild(eActionButton);
            #endregion           
            #endregion
            mainGameView = new ScaledViewPanel(this, Game, Controller);
            Timer.Interval = 1000 / 120;
            Timer.Tick += TimerTick;
            Timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var logicalMouse = mainGameView.MouseLogicalPos;
            Text = $"LogicalLocation: {MousePosition}, StepActionsLeft: {Game.PlayerScheduler.MaxActionCount - Game.PlayerScheduler.ActionCount}";

            foreach (var button in actionButtons)
            {
                button.Value.Enabled = Game.SelectedEntity != null;
                if (button.Value.Enabled && button.Key == Game.SelectedAttack)
                    button.Value.Select();
                else
                    button.Value.Deselect();
                actionBars[button.Key].Visible = actionBars[button.Key].CurrentValue != 0;
            }
            foreach (var (ent, b) in customButtons)
            {
                if (Game.SelectedEntity == ent)
                    b.Select();
                else
                    b.Deselect();
                b.Enabled = Game.IsAccessible && ent.IsAlive;
            }
            if (Game.CurrentLevel.InBounds(logicalMouse))
            {
                var obj = Game.CurrentLevel[logicalMouse].GameObjects.FirstOrDefault(x => x is Entity || x is CollectableGameObject);
                if (obj != null)
                {
                    tooltip.Location = MousePosition;
                    tooltip.Visible = true;
                    tooltip.TitleText = obj.Name;
                    tooltip.DescriptionText = obj.Description;
                    tooltip.IconImage = obj.Sprite;
                }
                else
                    tooltip.Visible = false;
            }
            Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(mainGameView.GetBitmap(), Point.Empty);
            hudContainer.Draw(e.Graphics);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Controller.HandleKeyPress(e.KeyCode);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            MousePosition = e.Location;
        }

        protected override void OnMouseClick(MouseEventArgs e) => ClickPerformed?.Invoke(this, new CustomMouseEventArg(e));
    }
}
