using System;
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Fingear.Utils;
using Glyph.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Core.ControlLayers
{
    [Flags]
    public enum MenuInputsType
    {
        Gamepad = 1 << 0,
        Mouse = 1 << 1,
        Keyboard = 1 << 2,
        All = Gamepad | Mouse | Keyboard
    }

    public class MenuControls : ControlLayerBase
    {
        private const float JoystickDeadZone = 0.5f;
        public IControl<InputActivity> Up { get; }
        public IControl<InputActivity> Down { get; }
        public IControl<InputActivity> Left { get; }
        public IControl<InputActivity> Right { get; }
        public IControl<InputActivity> Confirm { get; }
        public IControl<InputActivity> Cancel { get; }
        public IControl<InputActivity> Launch { get; }
        public IControl<InputActivity> Exit { get; }
        public IControl<InputActivity> Clic { get; }

        public MenuControls(MenuInputsType type, PlayerIndex playerIndex = PlayerIndex.One)
        {
            MonoGameInputSytem inputSystem = MonoGameInputSytem.Instance;

            var up = new ControlSet<InputActivity>("Up");
            var down = new ControlSet<InputActivity>("Down");
            var left = new ControlSet<InputActivity>("Left");
            var right = new ControlSet<InputActivity>("Right");
            var confirm = new ControlSet<InputActivity>("Confirm");
            var cancel = new ControlSet<InputActivity>("Cancel");
            var launch = new ControlSet<InputActivity>("Launch");
            var exit = new ControlSet<InputActivity>("Exit");
            var clic = new ControlSet<InputActivity>("Clic");

            if (type.HasFlag(MenuInputsType.Gamepad))
            {
                GamePadSource gamePad = inputSystem[PlayerIndex.One];

                up.Add(new ActivityControl(gamePad[GamePadButton.Up]));
                up.Add(new ActivityControl(gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Plus(Fingear.Converters.Axis.Y, JoystickDeadZone))));

                down.Add(new ActivityControl(gamePad[GamePadButton.Down]));
                down.Add(new ActivityControl(gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Minus(Fingear.Converters.Axis.Y, -JoystickDeadZone))));

                left.Add(new ActivityControl(gamePad[GamePadButton.Left]));
                left.Add(new ActivityControl(gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Plus(Fingear.Converters.Axis.X, -JoystickDeadZone))));

                right.Add(new ActivityControl(gamePad[GamePadButton.Right]));
                right.Add(new ActivityControl(gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Minus(Fingear.Converters.Axis.X, JoystickDeadZone))));

                confirm.Add(new ActivityControl(gamePad[GamePadButton.A]));

                cancel.Add(new ActivityControl(gamePad[GamePadButton.B]));
                cancel.Add(new ActivityControl(gamePad[GamePadButton.Back]));

                launch.Add(new ActivityControl(gamePad[GamePadButton.A]));
                launch.Add(new ActivityControl(gamePad[GamePadButton.Start]));

                exit.Add(new ActivityControl(gamePad[GamePadButton.Back]));
            }

            if (type.HasFlag(MenuInputsType.Mouse))
            {
                MouseSource mouse = inputSystem.Mouse;

                clic.Add(new ActivityControl(mouse[MouseButton.Left]));
            }

            if (type.HasFlag(MenuInputsType.Keyboard))
            {
                KeyboardSource keyboard = inputSystem.Keyboard;

                up.Add(new ActivityControl(keyboard[Keys.Up]));
                down.Add(new ActivityControl(keyboard[Keys.Down]));
                left.Add(new ActivityControl(keyboard[Keys.Left]));
                right.Add(new ActivityControl(keyboard[Keys.Right]));

                confirm.Add(new ActivityControl(keyboard[Keys.Enter]));
                confirm.Add(new ActivityControl(keyboard[Keys.Space]));

                cancel.Add(new ActivityControl(keyboard[Keys.Escape]));

                launch.Add(new ActivityControl(keyboard[Keys.Enter]));
                launch.Add(new ActivityControl(keyboard[Keys.Space]));

                exit.Add(new ActivityControl(keyboard[Keys.Escape]));
            }

            Add(Up = up);
            Add(Down = down);
            Add(Left = left);
            Add(Right = right);
            Add(Confirm = confirm);
            Add(Cancel = cancel);
            Add(Launch = launch);
            Add(Exit = exit);
            Add(Clic = clic);
        }
    }
}