using System.Collections.Generic;
using Glyph.Input.Composites;
using Glyph.Input.Handlers.Buttons;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardInputs
{
    public class MenuInputs : List<IInputHandler>
    {
        private const string Prefix = "menu-";
        public const string Up = Prefix + "Up";
        public const string Down = Prefix + "Down";
        public const string Left = Prefix + "Left";
        public const string Right = Prefix + "Right";
        public const string Confirm = Prefix + "Confirm";
        public const string Cancel = Prefix + "Cancel";
        public const string Launch = Prefix + "Launch";
        public const string Exit = Prefix + "Exit";
        public const string Clic = Prefix + "Clic";

        public MenuInputs(bool gamepad, bool mouse, bool keyboard)
        {
            var up = new InputSet<InputAction>(Up);
            var down = new InputSet<InputAction>(Down);
            var left = new InputSet<InputAction>(Left);
            var right = new InputSet<InputAction>(Right);
            var confirm = new InputSet<InputAction>(Confirm);
            var cancel = new InputSet<InputAction>(Cancel);
            var launch = new InputSet<InputAction>(Launch);
            var exit = new InputSet<InputAction>(Exit);
            var clic = new InputSet<InputAction>(Clic);

            if (gamepad)
            {
                up.Add(new PadButtonHandler("DPad", Buttons.DPadUp));
                up.Add(new PadButtonHandler("ThumbStick", Buttons.LeftThumbstickUp));

                down.Add(new PadButtonHandler("DPad", Buttons.DPadDown));
                down.Add(new PadButtonHandler("ThumbStick", Buttons.LeftThumbstickDown));

                left.Add(new PadButtonHandler("DPad", Buttons.DPadLeft));
                left.Add(new PadButtonHandler("ThumbStick", Buttons.LeftThumbstickLeft));

                right.Add(new PadButtonHandler("DPad", Buttons.DPadRight));
                right.Add(new PadButtonHandler("ThumbStick", Buttons.LeftThumbstickRight));

                //right.SensitivityPad = 0.5f;

                confirm.Add(new PadButtonHandler("Button", Buttons.A));

                cancel.Add(new PadButtonHandler("Button1", Buttons.B));
                cancel.Add(new PadButtonHandler("Button2", Buttons.Back));

                launch.Add(new PadButtonHandler("Button1", Buttons.A));
                launch.Add(new PadButtonHandler("Button2", Buttons.Start));

                exit.Add(new PadButtonHandler("Button", Buttons.Back));
            }

            if (mouse)
                clic.Add(new MouseButtonHandler("Clic", MouseButton.Left));

            if (keyboard)
            {
                up.Add(new KeyHandler("Key", Keys.Up));
                down.Add(new KeyHandler("Key", Keys.Down));
                left.Add(new KeyHandler("Key", Keys.Left));
                right.Add(new KeyHandler("Key", Keys.Right));

                confirm.Add(new KeyHandler("Key", Keys.Enter));
                confirm.Add(new KeyHandler("Key", Keys.Space));

                cancel.Add(new KeyHandler("Key", Keys.Escape));

                launch.Add(new KeyHandler("Key", Keys.Enter));
                launch.Add(new KeyHandler("Key", Keys.Space));

                exit.Add(new KeyHandler("Key", Keys.Escape));
            }

            Add(confirm);
            Add(cancel);
            Add(up);
            Add(down);
            Add(left);
            Add(right);
            Add(launch);
            Add(exit);
            Add(clic);
        }
    }
}