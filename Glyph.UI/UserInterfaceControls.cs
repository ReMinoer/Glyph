using Fingear.Controls;
using Fingear.Controls.Composites;
using Fingear.Controls.Containers;
using Fingear.Inputs;
using Fingear.Inputs.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Fingear.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace Glyph.UI
{
    public class UserInterfaceControls
    {
        static private UserInterfaceControls _instance;
        static public UserInterfaceControls Instance => _instance ?? (_instance = new UserInterfaceControls());

        private readonly KeyboardSource _keyboard;
        private readonly MouseSource _mouse;
        private readonly GamePadSource _gamePad;
        private const float JoystickDeadZone = 0.5f;

        public IControl<InputActivity> Touch => new MultiSourceControl<InputActivity>(nameof(Touch))
        {
            [InputSourceType.Mouse] = new ActivityControl(_mouse[MouseButton.Left])
        };

        public IControl<Vector2> Direction => new MultiSourceControl<Vector2>(nameof(Direction))
        {
            [InputSourceType.GamePad] = new ControlSet<Vector2>
            {
                Components =
                {
                    new Control<Vector2>((_gamePad[GamePadButton.Left], _gamePad[GamePadButton.Right], _gamePad[GamePadButton.Up], _gamePad[GamePadButton.Down]).Vector(-Vector2.One, Vector2.One)),
                    new Control<Vector2>(_gamePad[GamePadThumbstick.Left], x => DeadZone.VectorRadius(JoystickDeadZone)(x.Value))
                }
            },
            [InputSourceType.Keyboard] = new Control<Vector2>((_keyboard[Keys.Left], _keyboard[Keys.Right], _keyboard[Keys.Up], _keyboard[Keys.Down]).Vector(-Vector2.One, Vector2.One))
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

        public UserInterfaceControls()
        {
            InputSystem inputSystem = InputSystem.Instance;
            _keyboard = inputSystem.Keyboard;
            _mouse = inputSystem.Mouse;
            _gamePad = inputSystem[PlayerIndex.One];
        }
    }
}