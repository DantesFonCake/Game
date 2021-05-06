using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class CustomDrawableControl
    {
        private readonly GameWindow Parent;
        public event EventHandler Click;
        private bool enabled = true;

        #region Location and Size
        private Point location;
        private Size size;
        private int inactiveBorderWidth;
        private Rectangle clipRectangle;
        public Point Location
        {
            get => location;
            set
            {

                location = value;
                RecalculateClipRectangle();
            }

        }
        public Size Size
        {
            get => size;
            set
            {
                size = value;
                RecalculateClipRectangle();
            }
        }
        public int InactiveBorderWidth
        {
            get => inactiveBorderWidth;
            set
            {
                inactiveBorderWidth = value;
                RecalculateClipRectangle();
            }
        }

        private void RecalculateClipRectangle() =>
            clipRectangle = new Rectangle(Location + new Size(InactiveBorderWidth, InactiveBorderWidth), Size - 2 * new Size(InactiveBorderWidth, InactiveBorderWidth));

        public Rectangle ClipRectangle => clipRectangle;
        #endregion
        #region Position and Size Locking
        private bool locked = false;
        private float XLocationPercent;
        private float YLocationPercent;
        private float XSizePercent;
        private float YSizePercent;
        private float BorderWidthPercent;

        public void Lock()
        {
            XLocationPercent = (float)Location.X / Parent.ClientSize.Width;
            YLocationPercent = (float)Location.Y / Parent.ClientSize.Height;
            XSizePercent = (float)Size.Width / Parent.ClientSize.Width;
            YSizePercent = (float)Size.Height / Parent.ClientSize.Height;
            BorderWidthPercent = (float)inactiveBorderWidth / Parent.ClientSize.Height;
            locked = true;
        }
        private void OnParentResize()
        {
            if (locked)
            {
                Location = Point.Round(new PointF(Parent.ClientSize.Width * XLocationPercent, Parent.ClientSize.Height * YLocationPercent));
                Size = Size.Round(new SizeF(Parent.ClientSize.Width * XSizePercent, Parent.ClientSize.Height * YSizePercent));
                InactiveBorderWidth = (int)(Parent.ClientSize.Height * BorderWidthPercent);
            }
        }
        #endregion

        public bool Hovered
        {
            get;
            protected set;
        }
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (!enabled)
                    Deselect();
            }
        }
        public bool Selected { get; protected set; }
        public Bitmap Image { get; set; }
        public Color HoveredColor { get; protected set; } = Color.FromArgb(96, Color.Green);
        public Color SelectedColor { get; protected set; } = Color.FromArgb(96, Color.Yellow);
        public Color DisabledColor { get; protected set; } = Color.FromArgb(96, Color.Gray);

        private List<(Point location, (string text, Font font))> Texts { get; set; }
        public Brush TextBrush { get; set; } = Brushes.Black;

        public CustomDrawableControl(GameWindow parent)
        {
            Texts = new List<(Point location, (string text, Font font))>();
            Parent = parent;
            Parent.SizeChanged += (s, e) =>
            {
                OnParentResize();
            };
            Parent.MouseMove += (_, e) =>
            {
                Hovered = ClipRectangle.Contains(e.Location);
            };
            Parent.ClickPerformed += (_, e) =>
            {
                if (Hovered && Enabled && !e.Handled)
                {
                    e.Handled = true;
                    Click?.Invoke(this, new EventArgs());
                }
            };
        }

        public void Select() => Selected = true;
        public void Deselect() => Selected = false;

        public bool AddText(Point relativeLocation, string text, Font font)
        {
            //var location = Location + new Size(relativeLocation);
            //if (ClipRectangle.Contains(location)
            //    && ClipRectangle.Contains(
            //        location.X + (int)font.SizeInPoints * text.Length, location.Y))
            //{
            Texts.Add((relativeLocation, (text, font)));
            return true;
            //}
            return false;
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(Image, new Rectangle(Location, Size));
            if (Selected)
                g.FillRectangle(new SolidBrush(SelectedColor), ClipRectangle);
            if (Hovered && Enabled)
                g.FillRectangle(new SolidBrush(HoveredColor), ClipRectangle);
            if (!Enabled)
                g.FillRectangle(new SolidBrush(DisabledColor), ClipRectangle);
            foreach (var (l, (t, f)) in Texts)
            {
                var textSize = new SizeF(t.Length * f.SizeInPoints, f.SizeInPoints);
                g.DrawString(t, f, TextBrush, Location + new Size(l) - Size.Truncate(textSize/2));
            }
        }
    }
}
