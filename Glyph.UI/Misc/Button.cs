using System;
using Glyph.Input;
using Glyph.Input.StandardInputs;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Misc
{
    public class Button : TextSprite
    {
        public bool Enable { get; set; }
        public Padding Padding { get; set; }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public override sealed Vector2 Position
        {
            get { return base.Position; }
            set { base.Position = value; }
        }

        public override Rectangle Hitbox
        {
            get { return base.Hitbox.Padding(Padding); }
        }

        public event EventHandler Activated;
        public event EventHandler Hover;
        public event EventHandler Leave;

        public Button()
        {
        }

        public Button(string txt, int x, int y)
        {
            Text = txt;
            Position = new Vector2(x, y);
        }

        public override void Initialize()
        {
            base.Initialize();
            Enable = false;
        }

        public virtual void HandleInput(InputManager input)
        {
            if (input.IsMouseUsed)
            {
                var mouseInScreen = input.GetValue<Vector2>(MouseInputs.VirtualScreenPosition);
                bool enable = Hitbox.Contains(mouseInScreen);

                if (enable != Enable)
                {
                    Enable = enable;

                    if (enable && Hover != null)
                        Hover(this, EventArgs.Empty);
                    else if (!enable && Leave != null)
                        Leave(this, EventArgs.Empty);
                }
            }

            if (Enable)
            {
                if (input.IsMouseUsed && input[MenuInputs.Clic])
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);

                if (input[MenuInputs.Confirm])
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);
            }
        }
    }
}