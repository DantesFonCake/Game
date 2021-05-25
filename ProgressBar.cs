using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    public class ProgressBar : CustomDrawableComponent
    {
        Func<int> valueGetter;
        Brush brush;
        private Color color;

        public int MinimalValue { get; protected set; }
        public int MaximumValue { get; protected set; }
        public bool IsCompleted => MaximumValue == CurrentValue;
        public Color CompletedColor 
        { 
            get => color; 
            set
            {
                if(color!=value)
                    brush = new SolidBrush(color);
                color = value;
            } 
        }
        public int CurrentValue => valueGetter();
        public ProgressBar(GameWindow window, int minValue, int maxValue, Func<int> currentValueGetter) : base(window)
        {
            MinimalValue = minValue;
            MaximumValue = maxValue;
            valueGetter = currentValueGetter;
        }

        public void ChangeValueGetter(Func<int> newGetter)
        {
            valueGetter = newGetter;
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (!Visible)
                return;
            var percent = (double)(CurrentValue - MinimalValue) / (MaximumValue - MinimalValue);
            g.FillRectangle(brush, new Rectangle(ClipRectangle.Location, ClipRectangle.Size - new Size((int)((1-percent)*ClipRectangle.Width), 0)));
        }
    }
}
