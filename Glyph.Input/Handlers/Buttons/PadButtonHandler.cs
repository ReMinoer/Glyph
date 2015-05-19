using Microsoft.Xna.Framework;
using XnaButtons = Microsoft.Xna.Framework.Input.Buttons;

namespace Glyph.Input.Handlers.Buttons
{
    public class PadButtonHandler : ButtonHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public XnaButtons Button { get; set; }

        public PadButtonHandler(string name, XnaButtons button, PlayerIndex playerIndex = PlayerIndex.One,
            InputAction desiredAction = InputAction.Triggered)
            : base(name, desiredAction)
        {
            PlayerIndex = playerIndex;
            Button = button;
        }

        public PadButtonHandler(string name, XnaButtons button, InputAction mode)
            : this(name, button, PlayerIndex.One, mode)
        {
        }

        public PadButtonHandler(string name, XnaButtons button, PlayerIndex playerIndex)
            // ReSharper disable once RedundantArgumentDefaultValue
            : this(name, button, playerIndex, InputAction.Triggered)
        {
        }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        protected override bool GetState(InputManager inputManager)
        {
            return inputManager.GamePadStates[PlayerIndex].IsButtonDown(Button);
        }
    }
}