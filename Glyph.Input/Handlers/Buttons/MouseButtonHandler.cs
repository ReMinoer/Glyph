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

        protected override bool GetState(InputStates inputStates)
        {
            switch (MouseBoutton)
            {
                case MouseButton.Left:
                    return inputStates.MouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return inputStates.MouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return inputStates.MouseState.MiddleButton == ButtonState.Pressed;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}