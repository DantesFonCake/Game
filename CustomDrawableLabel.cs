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

        public Color TextColor { get; set; }
        public string Text;
        public Font Font { get; set; }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (!Visible)
                return;
            g.DrawString(Text, Font, new SolidBrush(TextColor), new RectangleF(Location,Size));
        }
    }
}
