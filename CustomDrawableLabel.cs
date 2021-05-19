using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Game
{
    public class CustomDrawableLabel : CustomDrawableComponent
    {
        public CustomDrawableLabel(GameWindow window) : base(window)
        {
            TextColor = Color.Black;
        }

        public Color TextColor 
        {
            get => color;
            set
            {
                color = value;
                textBrush = new SolidBrush(color);
            }
        }
        public string Text;
        public Font Font
        {
            get => font;
            set
            {
                font = value;
                fontSizePecent = (float)font.SizeInPoints / ParentSize.Width;
            }
        }
        float fontSizePecent;
        private Font font;
        private Color color;
        private Brush textBrush;

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            var textSize = new SizeF(Text.Length * Font.SizeInPoints, Font.SizeInPoints);
            var textLocation = Location;//new Point(Location); //+ Size.Truncate(textSize / 2));
            g.DrawString(Text, Font, textBrush, textLocation);
        }
    }
}
