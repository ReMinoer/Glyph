using System;
using Glyph.Input;
using Glyph.Input.StandardActions;
using Microsoft.Xna.Framework;

namespace Glyph.UI.Controls
{
    public class Button : TextSprite, IHandleInput
    {
        public bool Enable { get; set; }
        public virtual Padding Padding { get; set; }
        public override Rectangle Hitbox { get { return base.Hitbox.Padding(Padding); } }
        public event EventHandler Activated;
        public event EventHandler Hover;
        public event EventHandler Leave;

        public override void Initialize()
        {
            base.Initialize();
            Reset();
        }

        public override void Initialize(string txt, int x, int y)
        {
            base.Initialize(txt, x, y);
            Reset();
        }

        public virtual void Reset()
        {
            Enable = false;
        }

        public virtual void HandleInput(InputManager input)
        {
            if (input.IsMouseUsed)
            {
                bool enable = Hitbox.Contains((int)input.MouseScreen.X, (int)input.MouseScreen.Y);

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
                if (input.IsMouseUsed && input.IsActionDownNow(MenuActions.Clic))
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);

                if (input.IsActionDownNow(MenuActions.Confirm))
                    if (Activated != null)
                        Activated(this, EventArgs.Empty);
            }
        }
    }
}