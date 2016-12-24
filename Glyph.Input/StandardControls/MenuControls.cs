using System;   
using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Converters;
using Fingear.MonoGame.Inputs;
using Fingear.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardControls
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
                up.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Up, playerIndex)));
                up.Add(new ActivityControl(new GamePadThumbstickInput(GamePadThumbstick.Left, playerIndex).Boolean(DeadZone.Plus(Fingear.Converters.Axis.Y, JoystickDeadZone))));

                down.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Down, playerIndex)));
                down.Add(new ActivityControl(new GamePadThumbstickInput(GamePadThumbstick.Left, playerIndex).Boolean(DeadZone.Minus(Fingear.Converters.Axis.Y, -JoystickDeadZone))));

                left.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Left, playerIndex)));
                left.Add(new ActivityControl(new GamePadThumbstickInput(GamePadThumbstick.Left, playerIndex).Boolean(DeadZone.Plus(Fingear.Converters.Axis.X, -JoystickDeadZone))));

                right.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Right, playerIndex)));
                right.Add(new ActivityControl(new GamePadThumbstickInput(GamePadThumbstick.Left, playerIndex).Boolean(DeadZone.Minus(Fingear.Converters.Axis.X, JoystickDeadZone))));

                confirm.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.A, playerIndex)));

                cancel.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.B, playerIndex)));
                cancel.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Back, playerIndex)));

                launch.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.A, playerIndex)));
                launch.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Start, playerIndex)));

                exit.Add(new ActivityControl(new GamePadButtonInput(GamePadButton.Back, playerIndex)));
            }

            if (type.HasFlag(MenuInputsType.Mouse))
            {
                clic.Add(new ActivityControl(new MouseButtonInput(MouseButton.Left)));
            }

            if (type.HasFlag(MenuInputsType.Keyboard))
            {
                up.Add(new ActivityControl(new KeyInput(Keys.Up)));
                down.Add(new ActivityControl(new KeyInput(Keys.Down)));
                left.Add(new ActivityControl(new KeyInput(Keys.Left)));
                right.Add(new ActivityControl(new KeyInput(Keys.Right)));

                confirm.Add(new ActivityControl(new KeyInput(Keys.Enter)));
                confirm.Add(new ActivityControl(new KeyInput(Keys.Space)));

                cancel.Add(new ActivityControl(new KeyInput(Keys.Escape)));

                launch.Add(new ActivityControl(new KeyInput(Keys.Enter)));
                launch.Add(new ActivityControl(new KeyInput(Keys.Space)));

                exit.Add(new ActivityControl(new KeyInput(Keys.Escape)));
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