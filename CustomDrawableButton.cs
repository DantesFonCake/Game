using System;
using System.Drawing;

namespace Game
{
    public class CustomDrawableButton : CustomDrawableComponent
    {
        public event EventHandler Click;
        public Color SelectedColor { get; set; } = Color.Transparent;
        public bool Selected { get; protected set; }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (!Enabled)
                    Deselect();
            }
        }
        public void Select() => Selected = true;
        public void Deselect() => Selected = false;

        public CustomDrawableButton(GameWindow window) : base(window)
        {
            
        }

        protected override void ReboundParent(CustomDrawableComponent parent)
        {
            if (parent == null)
            {
                if(Parent!=null)
                    Parent.ClickPerformed -= OnClick;
                Window.ClickPerformed += OnClick;
            }
            else
            {
                Window.ClickPerformed -= OnClick;
                parent.ClickPerformed += OnClick;
            }
            base.ReboundParent(parent);        
        }

        private void OnClick(object sender,CustomMouseEventArg e)
        {
            if (Hovered && Enabled && !e.Handled)
            {
                e.Handled = true;
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (!Visible)
                return;
            if (Selected)
                g.FillRectangle(new SolidBrush(SelectedColor), ClipRectangle);
        }
    }
}
