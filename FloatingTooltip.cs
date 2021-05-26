using System.Drawing;

namespace Game
{
    public class FloatingTooltip : CustomDrawableComponent
    {
        private readonly CustomDrawableComponent Icon;
        private readonly CustomDrawableLabel Title;
        private readonly CustomDrawableLabel Description;
        private Font font;
        private readonly int whiteSpace = 5;

        public Font Font
        {
            get => font;
            set
            {
                font = value;
                Title.Font = Font;
                Description.Font = Font;
            }
        }

        public string TitleText
        {
            get => Title.Text;
            set => Title.Text = value;
        }

        public string DescriptionText
        {
            get => Description.Text;
            set => Description.Text = value;
        }

        public Bitmap IconImage
        {
            get => Icon.Image;
            set => Icon.Image = value;
        }

        public FloatingTooltip(GameWindow window, int fontSize) : base(window)
        {
            Size = new Size(96*3, 96 * 4);
            Icon = new CustomDrawableComponent(window)
            {
                Location = new Point(whiteSpace, whiteSpace),
                Size = new Size(30, 30),
                InactiveBorderWidth = 0,
                //BackColor = ColorTranslator.FromHtml("#06360a"),
            };
            Title = new CustomDrawableLabel(window)
            {
                Location = new Point(2*whiteSpace + 30, whiteSpace),
                Size = new Size(248, 30),
                //BackColor = ColorTranslator.FromHtml("#06360a"),
            };
            Description = new CustomDrawableLabel(window)
            {
                Location = new Point(whiteSpace, 40),
                Size = new Size(288-2*whiteSpace, 157),
                //BackColor = ColorTranslator.FromHtml("#06360a"),
            };
            AddChild(Description);
            AddChild(Title);
            AddChild(Icon);
            Description.Lock();
            Title.Lock();
            Icon.Lock();

        }
    }
}
