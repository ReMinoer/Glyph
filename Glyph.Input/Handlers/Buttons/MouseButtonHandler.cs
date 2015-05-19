using System;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.Handlers.Buttons
{
    public class MouseButtonHandler : ButtonHandler
    {
        public MouseButton MouseBoutton { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.Mouse; }
        }

        public MouseButtonHandler(string name, MouseButton mouseBoutton,
            InputAction desiredAction = InputAction.Triggered)
            : base(name, desiredAction)
        {
            MouseBoutton = mouseBoutton;
        }

        protected override bool GetState(InputManager inputManager)
        {
            switch (MouseBoutton)
            {
                case MouseButton.Left:
                    return inputManager.MouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return inputManager.MouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return inputManager.MouseState.MiddleButton == ButtonState.Pressed;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}