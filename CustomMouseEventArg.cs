using System.Drawing;
using System.Windows.Forms;

namespace Game
{
    public class CustomMouseEventArg : CustomEventArgs
    {
        public Point Location { get; }
        public MouseButtons Button { get; }

        public CustomMouseEventArg(System.Windows.Forms.MouseEventArgs args)
        {
            Location = args.Location;
            Button = args.Button;
        }
    }
}