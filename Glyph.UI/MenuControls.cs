using Fingear;
using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Controls.Containers;
using Fingear.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Fingear.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Glyph.UI
{
    public class MenuControls
    {
        static private MenuControls _instance;
        static public MenuControls Instance => _instance ?? (_instance = new MenuControls());

        private readonly KeyboardSource _keyboard;
        private readonly MouseSource _mouse;
        private readonly GamePadSource _gamePad;
        private const float JoystickDeadZone = 0.5f;

        public IControl<InputActivity> Up => new MultiSourceControl<InputActivity>(nameof(Up))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.Up]),
                    new ActivityControl(_gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Plus(Fingear.Converters.Axis.Y, JoystickDeadZone)))
                }
            },
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Up])
        };

        public IControl<InputActivity> Down => new MultiSourceControl<InputActivity>(nameof(Down))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.Down]),
                    new ActivityControl(_gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Minus(Fingear.Converters.Axis.Y, -JoystickDeadZone)))
                }
            },
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Down])
        };

        public IControl<InputActivity> Left => new MultiSourceControl<InputActivity>(nameof(Left))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.Left]),
                    new ActivityControl(_gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Plus(Fingear.Converters.Axis.X, JoystickDeadZone)))
                }
            },
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Left])
        };

        public IControl<InputActivity> Right => new MultiSourceControl<InputActivity>(nameof(Right))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.Right]),
                    new ActivityControl(_gamePad[GamePadThumbstick.Left].Boolean(DeadZone.Minus(Fingear.Converters.Axis.X, -JoystickDeadZone)))
                }
            },
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Right])
        };

        public IControl<InputActivity> Confirm => new MultiSourceControl<InputActivity>(nameof(Confirm))
        {
            [InputSourceType.GamePad] = new ActivityControl(_gamePad[GamePadButton.A]),
            [InputSourceType.Keyboard] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_keyboard[Keys.Enter]),
                    new ActivityControl(_keyboard[Keys.Space])
                }
            }
        };

        public IControl<InputActivity> Launch => new MultiSourceControl<InputActivity>(nameof(Launch))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.A]),
                    new ActivityControl(_gamePad[GamePadButton.Start])
                }
            },
            [InputSourceType.Keyboard] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_keyboard[Keys.Enter]),
                    new ActivityControl(_keyboard[Keys.Space])
                }
            }
        };

        public IControl<InputActivity> Cancel => new MultiSourceControl<InputActivity>(nameof(Cancel))
        {
            [InputSourceType.GamePad] = new ControlSet<InputActivity>
            {
                Components =
                {
                    new ActivityControl(_gamePad[GamePadButton.B]),
                    new ActivityControl(_gamePad[GamePadButton.Back])
                }
            },
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Escape])
        };

        public IControl<InputActivity> Exit => new MultiSourceControl<InputActivity>(nameof(Exit))
        {
            [InputSourceType.GamePad] = new ActivityControl(_gamePad[GamePadButton.Back]),
            [InputSourceType.Keyboard] = new ActivityControl(_keyboard[Keys.Escape])
        };

        public IControl<InputActivity> Clic => new MultiSourceControl<InputActivity>(nameof(Clic))
        {
            [InputSourceType.Mouse] = new ActivityControl(_mouse[MouseButton.Left])
        };

        public IControl<InputActivity> ContextMenu => new MultiSourceControl<InputActivity>(nameof(ContextMenu))
        {
            [InputSourceType.Mouse] = new ActivityControl(_mouse[MouseButton.Right])
        };

        public MenuControls()
        {
            InputSystem inputSystem = InputSystem.Instance;
            _keyboard = inputSystem.Keyboard;
            _mouse = inputSystem.Mouse;
            _gamePad = inputSystem[PlayerIndex.One];
        }
    }
}