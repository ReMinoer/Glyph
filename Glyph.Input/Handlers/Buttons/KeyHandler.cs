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

        public KeyHandler(string name, Keys key, InputAction desiredAction = InputAction.Triggered)
            : base(name, desiredAction)
        {
            Key = key;
        }

        protected override bool GetState(InputStates inputStates)
        {
            return inputStates.KeyboardState.IsKeyDown(Key);
        }
    }
}