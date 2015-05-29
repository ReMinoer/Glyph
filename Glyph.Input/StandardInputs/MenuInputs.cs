using System.Collections.Generic;
using Glyph.Input.Composites;
using Glyph.Input.Converters;
using Glyph.Input.Handlers.Axis;
using Glyph.Input.Handlers.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardInputs
{
    public class MenuInputs : List<IInputHandler>
    {
        private const string Prefix = "Menu-";
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
            var up = new InputSet<InputActivity>(Up);
            var down = new InputSet<InputActivity>(Down);
            var left = new InputSet<InputActivity>(Left);
            var right = new InputSet<InputActivity>(Right);
            var confirm = new InputSet<InputActivity>(Confirm);
            var cancel = new InputSet<InputActivity>(Cancel);
            var launch = new InputSet<InputActivity>(Launch);
            var exit = new InputSet<InputActivity>(Exit);
            var clic = new InputSet<InputActivity>(Clic);

            if (gamepad)
            {
                up.Add(new PadButtonHandler("DPad", Buttons.DPadUp));
                up.Add(new ThumbStickAxisHandler("ThumbStick", ThumbStick.Left, Axis.Vertical, 0.5f, AxisSign.Positive, InputActivity.Triggered));

                down.Add(new PadButtonHandler("DPad", Buttons.DPadDown));
                down.Add(new ThumbStickAxisHandler("ThumbStick", ThumbStick.Left, Axis.Vertical, 0.5f, AxisSign.Negative, InputActivity.Triggered));

                left.Add(new PadButtonHandler("DPad", Buttons.DPadLeft));
                left.Add(new ThumbStickAxisHandler("ThumbStick", ThumbStick.Left, Axis.Horizontal, 0.5f, AxisSign.Negative, InputActivity.Triggered));

                right.Add(new PadButtonHandler("DPad", Buttons.DPadRight));
                right.Add(new ThumbStickAxisHandler("ThumbStick", ThumbStick.Left, Axis.Horizontal, 0.5f, AxisSign.Positive, InputActivity.Triggered));

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