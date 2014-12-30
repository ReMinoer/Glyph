using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input
{
    public class ActionButton : IActionButton
    {
        public List<Keys> Keys { get; set; }
        public List<Buttons> Buttons { get; set; }
        public bool ClicLeft { get; set; }
        public bool ClicRight { get; set; }
        public Orientation PadLeft { get; set; }
        public Orientation PadRight { get; set; }
        public bool TriggerLeft { get; set; }
        public bool TriggerRight { get; set; }

        public float SensitivityPad { get; set; }
        public float SensitivityTrigger { get; set; }

        private const float SensitivityPadByDefault = 0.1f;
        private const float SensitivityTriggerByDefault = 0.1f;

        public ActionButton(string name)
        {
            Name = name;

            Keys = new List<Keys>();
            Buttons = new List<Buttons>();
            ClicLeft = false;
            ClicRight = false;
            PadLeft = Orientation.None;
            PadRight = Orientation.None;
            TriggerLeft = false;
            TriggerRight = false;
            SensitivityPad = SensitivityPadByDefault;
            SensitivityTrigger = SensitivityTriggerByDefault;
        }

        public string Name { get; set; }

        public bool IsPressed(InputManager input)
        {
            return (Keys.Any() && IsKeyPressed(input)) || (Buttons.Any() && IsButtonPressed(input))
                   || (ClicLeft && IsClicLeftPressed(input)) || (ClicRight && IsClicRightPressed(input))
                   || (IsPadLeftPressed(input)) || (IsPadRightPressed(input))
                   || (TriggerLeft && IsTriggerLeftPressed(input)) || (TriggerRight && IsTriggerRightPressed(input));
        }

        public bool IsDownNow(InputManager input)
        {
            return (Keys.Any() && IsKeyDownNow(input)) || (Buttons.Any() && IsButtonDownNow(input))
                   || (ClicLeft && IsClicLeftNow(input)) || (ClicRight && IsClicRightNow(input))
                   || (IsPadLeftNow(input)) || (IsPadRightNow(input)) || (TriggerLeft && IsTriggerLeftNow(input))
                   || (TriggerRight && IsTriggerRightNow(input));
        }

        public bool IsKeyPressed(InputManager input)
        {
            foreach (Keys k in Keys)
                if (input.Keyboard.IsKeyDown(k))
                    return true;
            return false;
        }

        public bool IsButtonPressed(InputManager input)
        {
            foreach (Buttons b in Buttons)
                if (input.GamePad.IsButtonDown(b))
                    return true;
            return false;
        }

        static public bool IsClicLeftPressed(InputManager input)
        {
            return input.Mouse.LeftButton == ButtonState.Pressed;
        }

        static public bool IsClicRightPressed(InputManager input)
        {
            return input.Mouse.RightButton == ButtonState.Pressed;
        }

        public bool IsPadLeftPressed(InputManager input)
        {
            switch (PadLeft)
            {
                case Orientation.Up:
                    return input.GamePad.ThumbSticks.Left.Y >= SensitivityPad;

                case Orientation.Right:
                    return input.GamePad.ThumbSticks.Left.X >= SensitivityPad;

                case Orientation.Down:
                    return input.GamePad.ThumbSticks.Left.Y <= -SensitivityPad;

                case Orientation.Left:
                    return input.GamePad.ThumbSticks.Left.X <= -SensitivityPad;

                default:
                    return false;
            }
        }

        public bool IsPadRightPressed(InputManager input)
        {
            switch (PadRight)
            {
                case Orientation.Up:
                    return input.GamePad.ThumbSticks.Right.Y >= SensitivityPad;

                case Orientation.Right:
                    return input.GamePad.ThumbSticks.Right.X >= SensitivityPad;

                case Orientation.Down:
                    return input.GamePad.ThumbSticks.Right.Y <= -SensitivityPad;

                case Orientation.Left:
                    return input.GamePad.ThumbSticks.Right.X <= -SensitivityPad;

                default:
                    return false;
            }
        }

        public bool IsTriggerLeftPressed(InputManager input)
        {
            return input.GamePad.Triggers.Left >= SensitivityTrigger;
        }

        public bool IsTriggerRightPressed(InputManager input)
        {
            return input.GamePad.Triggers.Right >= SensitivityTrigger;
        }

        public bool IsKeyDownNow(InputManager input)
        {
            foreach (Keys k in Keys)
                if (input.Keyboard.IsKeyDown(k) && !input.LastKeyboard.IsKeyDown(k))
                    return true;
            return false;
        }

        public bool IsButtonDownNow(InputManager input)
        {
            foreach (Buttons b in Buttons)
                if (input.GamePad.IsButtonDown(b) && !input.LastGamePad.IsButtonDown(b))
                    return true;
            return false;
        }

        static public bool IsClicLeftNow(InputManager input)
        {
            return input.Mouse.LeftButton == ButtonState.Pressed && input.LastMouse.LeftButton == ButtonState.Released;
        }

        static public bool IsClicRightNow(InputManager input)
        {
            return input.Mouse.RightButton == ButtonState.Pressed && input.LastMouse.RightButton == ButtonState.Released;
        }

        public bool IsPadLeftNow(InputManager input)
        {
            switch (PadLeft)
            {
                case Orientation.Up:
                    return input.GamePad.ThumbSticks.Left.Y >= SensitivityPad
                           && input.LastGamePad.ThumbSticks.Left.Y < SensitivityPad;

                case Orientation.Right:
                    return input.GamePad.ThumbSticks.Left.X >= SensitivityPad
                           && input.LastGamePad.ThumbSticks.Left.X < SensitivityPad;

                case Orientation.Down:
                    return input.GamePad.ThumbSticks.Left.Y <= -SensitivityPad
                           && input.LastGamePad.ThumbSticks.Left.Y > -SensitivityPad;

                case Orientation.Left:
                    return input.GamePad.ThumbSticks.Left.X <= -SensitivityPad
                           && input.LastGamePad.ThumbSticks.Left.X > -SensitivityPad;

                default:
                    return false;
            }
        }

        public bool IsPadRightNow(InputManager input)
        {
            switch (PadRight)
            {
                case Orientation.Up:
                    return input.GamePad.ThumbSticks.Right.Y >= SensitivityPad
                           && input.LastGamePad.ThumbSticks.Right.Y < SensitivityPad;

                case Orientation.Right:
                    return input.GamePad.ThumbSticks.Right.X >= SensitivityPad
                           && input.LastGamePad.ThumbSticks.Right.X < SensitivityPad;

                case Orientation.Down:
                    return input.GamePad.ThumbSticks.Right.Y <= -SensitivityPad
                           && input.LastGamePad.ThumbSticks.Right.Y > -SensitivityPad;

                case Orientation.Left:
                    return input.GamePad.ThumbSticks.Right.X <= -SensitivityPad
                           && input.LastGamePad.ThumbSticks.Right.X > -SensitivityPad;

                default:
                    return false;
            }
        }

        public bool IsTriggerLeftNow(InputManager input)
        {
            return input.GamePad.Triggers.Left >= SensitivityTrigger
                   && input.LastGamePad.Triggers.Left < SensitivityTrigger;
        }

        public bool IsTriggerRightNow(InputManager input)
        {
            return input.GamePad.Triggers.Right >= SensitivityTrigger
                   && input.LastGamePad.Triggers.Right < SensitivityTrigger;
        }
    }
}