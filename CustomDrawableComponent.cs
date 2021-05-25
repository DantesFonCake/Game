using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game
{
    public class CustomDrawableComponent
    {
        protected readonly GameWindow Window;
        protected Size ParentSize { get; set; }
        public CustomDrawableComponent Parent
        {
            get => parent;
            protected set
            {
                parent = value;
                ParentSize = parent == null ? Window.ClientSize : parent.Size;
            }
        }
        private readonly List<CustomDrawableComponent> components;

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
        private CustomDrawableComponent parent;

        public void Lock()
        {
            XLocationPercent = (float)Location.X / ParentSize.Width;
            YLocationPercent = (float)Location.Y / ParentSize.Height;
            XSizePercent = (float)Size.Width / ParentSize.Width;
            YSizePercent = (float)Size.Height / ParentSize.Height;
            BorderWidthPercent = (float)inactiveBorderWidth / ParentSize.Height;
            locked = true;
        }
        #endregion

        public event EventHandler<CustomMouseEventArg> MouseMove;

        public virtual void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e) => OnMouseMove(this, new CustomMouseEventArg(e));

        public virtual void OnMouseMove(object sender, CustomMouseEventArg e)
        {
            MouseMove?.Invoke(this, e);
            Hovered = ClipRectangle.Contains(e.Location);
        }

        public event EventHandler SizeChanged;

        public void OnParentResize(object sender, EventArgs e) => OnParentResize();

        public event EventHandler<CustomMouseEventArg> ClickPerformed;

        public void OnClickPerformed(object sender, CustomMouseEventArg e) => ClickPerformed?.Invoke(this, e);
        private void OnParentResize()
        {
            if (locked)
            {
                Location = Point.Round(new PointF(ParentSize.Width * XLocationPercent, ParentSize.Height * YLocationPercent));
                Size = Size.Round(new SizeF(ParentSize.Width * XSizePercent, ParentSize.Height * YSizePercent));
                InactiveBorderWidth = (int)(ParentSize.Height * BorderWidthPercent);
                SizeChanged?.Invoke(this, EventArgs.Empty);

            }
        }

        public virtual bool Hovered { get; protected set; }
        public virtual bool Enabled { get; set; }
        public virtual bool Visible { get; set; } = true;
        public Bitmap Image { get; set; }
        public Color HoveredColor { get; set; } = Color.Transparent;
        public Color DisabledColor { get; set; } = Color.Transparent;

        protected IReadOnlyList<CustomDrawableComponent> Components => components;

        public CustomDrawableComponent(GameWindow window)
        {
            components = new List<CustomDrawableComponent>();
            Window = window;
            ReboundParent(parent);
        }

        protected virtual void ReboundParent(CustomDrawableComponent parent)
        {
            if (parent == null)
            {
                if (Parent != null)
                {
                    Parent.SizeChanged -= OnParentResize;
                    Parent.MouseMove -= OnMouseMove;
                    Parent.ClickPerformed -= OnClickPerformed;
                }
                Window.SizeChanged += OnParentResize;
                Window.MouseMove += OnMouseMove;
                Window.ClickPerformed += OnClickPerformed;
            }
            else
            {
                parent.SizeChanged += OnParentResize;
                parent.MouseMove += OnMouseMove;
                parent.ClickPerformed += OnClickPerformed;
            }
            Parent = parent;
        }

        public void AddChild(CustomDrawableComponent child)
        {
            child.ReboundParent(this);
            components.Add(child);
        }

        public virtual void Draw(Graphics g)
        {
            if (!Visible)
                return;
            var image = Image == null ? new Bitmap(Size.Width, Size.Height) : new Bitmap(Image,Size);
            var drawRectangle = new Rectangle(new Point(InactiveBorderWidth, InactiveBorderWidth), ClipRectangle.Size);
            using var graph = Graphics.FromImage(image);
            if (Enabled && Hovered)
                graph.FillRectangle(new SolidBrush(HoveredColor), drawRectangle);
            if (!Enabled)
                graph.FillRectangle(new SolidBrush(DisabledColor), drawRectangle);
            foreach (var item in Components)
            {
                item.Draw(graph);
            }
            g.DrawImage(image, new Rectangle(Location, Size));
        }
    }
}
