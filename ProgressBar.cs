using System;
using System.Drawing;

namespace Game
{
    public class ProgressBar : CustomDrawableComponent
    {
        public Func<int> ValueGetter { get; set; }
        public int MinimalValue { get; protected set; }
        public int MaximumValue { get; protected set; }
        public bool IsCompleted => MaximumValue == CurrentValue;
        public Color CompletedColor { get; set; }
        public int CurrentValue => ValueGetter?.Invoke() ?? MinimalValue;
        public ProgressBar(GameWindow window, int minValue, int maxValue, Func<int> currentValueGetter) : base(window)
        {
            MinimalValue = minValue;
            MaximumValue = maxValue;
            ValueGetter = currentValueGetter;
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (!Visible)
                return;
            var percent = (double)(CurrentValue - MinimalValue) / (MaximumValue - MinimalValue);
            percent = percent > 1 ? 1 : percent;
            var drawPosition = Parent != null ? new Point(Parent.InactiveBorderWidth, Parent.InactiveBorderWidth) : Point.Empty;
            var drawRectangle = new Rectangle(drawPosition, ClipRectangle.Size - new Size((int)((1 - percent) * ClipRectangle.Width), 0));

            g.FillRectangle(new SolidBrush(CompletedColor), drawRectangle);
        }
    }
}
