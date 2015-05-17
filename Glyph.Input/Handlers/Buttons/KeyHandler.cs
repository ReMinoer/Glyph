using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.Handlers.Buttons
{
    public class KeyHandler : ButtonHandler
    {
        public Keys Key { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.Keyboard; }
        }

        public KeyHandler(string name, Keys key, ButtonHandlerMode mode = ButtonHandlerMode.Triggered)
            : base(name, mode)
        {
            Key = key;
        }

        protected override bool GetState(InputManager inputManager)
        {
            return inputManager.KeyboardState.IsKeyDown(Key);
        }
    }
}