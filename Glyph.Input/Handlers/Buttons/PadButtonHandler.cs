using Microsoft.Xna.Framework;
using XnaButtons = Microsoft.Xna.Framework.Input.Buttons;

namespace Glyph.Input.Handlers.Buttons
{
    public class PadButtonHandler : ButtonHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public XnaButtons? Button { get; set; }

        public PadButtonHandler()
            : this("", null)
        {
        }

        public PadButtonHandler(string name, XnaButtons? button, InputAction desiredAction = InputAction.Triggered)
            : this(name, PlayerIndex.One, button, desiredAction)
        {
        }

        public PadButtonHandler(string name, PlayerIndex playerIndex, XnaButtons? button,
            InputAction desiredAction = InputAction.Triggered)
            : base(name, desiredAction)
        {
            PlayerIndex = playerIndex;
            Button = button;
        }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        protected override bool GetState(InputStates inputStates)
        {
            return Button.HasValue && inputStates.GamePadStates[PlayerIndex].IsButtonDown(Button.Value);
        }
    }
}