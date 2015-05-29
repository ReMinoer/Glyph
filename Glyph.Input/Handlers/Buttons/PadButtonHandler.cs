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

        public PadButtonHandler(string name, XnaButtons? button, InputActivity desiredActivity = InputActivity.Triggered)
            : this(name, PlayerIndex.One, button, desiredActivity)
        {
        }

        public PadButtonHandler(string name, PlayerIndex playerIndex, XnaButtons? button,
            InputActivity desiredActivity = InputActivity.Triggered)
            : base(name, desiredActivity)
        {
            PlayerIndex = playerIndex;
            Button = button;
        }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        protected override bool GetActivity(InputStates inputStates)
        {
            return Button.HasValue && inputStates.GamePadStates[PlayerIndex].IsButtonDown(Button.Value);
        }
    }
}