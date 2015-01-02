using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardActions
{
    public class MenuActions : ActionsCollection
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

        public MenuActions(bool gamepad, bool mouse, bool keyboard)
        {
            var up = new ActionButton(Up);
            var down = new ActionButton(Down);
            var left = new ActionButton(Left);
            var right = new ActionButton(Right);
            var confirm = new ActionButton(Confirm);
            var cancel = new ActionButton(Cancel);
            var launch = new ActionButton(Launch);
            var exit = new ActionButton(Exit);
            var clic = new ActionButton(Clic);

            if (gamepad)
            {
                up.Buttons.Add(Buttons.DPadUp);
                up.PadLeft = Orientation.Up;
                up.SensitivityPad = 0.5f;

                down.Buttons.Add(Buttons.DPadDown);
                down.PadLeft = Orientation.Down;
                down.SensitivityPad = 0.5f;

                left.Buttons.Add(Buttons.DPadLeft);
                left.PadLeft = Orientation.Left;
                left.SensitivityPad = 0.5f;

                right.Buttons.Add(Buttons.DPadRight);
                right.PadLeft = Orientation.Right;
                right.SensitivityPad = 0.5f;

                confirm.Buttons.Add(Buttons.A);

                cancel.Buttons.Add(Buttons.B);
                cancel.Buttons.Add(Buttons.Back);

                launch.Buttons.Add(Buttons.A);
                launch.Buttons.Add(Buttons.Start);

                exit.Buttons.Add(Buttons.Back);
            }

            if (mouse)
                clic.ClicLeft = true;

            if (keyboard)
            {
                up.Keys.Add(Keys.Up);
                down.Keys.Add(Keys.Down);
                left.Keys.Add(Keys.Left);
                right.Keys.Add(Keys.Right);

                confirm.Keys.Add(Keys.Enter);
                confirm.Keys.Add(Keys.Space);

                cancel.Keys.Add(Keys.Escape);

                launch.Keys.Add(Keys.Enter);
                launch.Keys.Add(Keys.Space);

                exit.Keys.Add(Keys.Escape);
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