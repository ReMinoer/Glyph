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

        public KeyHandler()
            : this("", Keys.None)
        {
        }

        public KeyHandler(string name, Keys key, InputActivity desiredActivity = InputActivity.Triggered)
            : base(name, desiredActivity)
        {
            Key = key;
        }

        protected override bool GetActivity(InputStates inputStates)
        {
            return Key != Keys.None && inputStates.KeyboardState.IsKeyDown(Key);
        }
    }
}