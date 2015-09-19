using System;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Axis
{
    public class PadTriggerHandler : AxisHandler
    {
        public PlayerIndex PlayerIndex { get; set; }
        public Trigger Trigger { get; set; }

        public override InputSource InputSource
        {
            get { return InputSource.GamePad; }
        }

        public PadTriggerHandler()
            : this("", Trigger.None)
        {
        }

        public PadTriggerHandler(string name, Trigger trigger, float deadZone = 0, AxisSign sign = AxisSign.None,
            bool inverse = false,
            InputActivity desiredActivity = InputActivity.Pressed)
            : this(name, PlayerIndex.One, trigger, deadZone, sign, inverse, desiredActivity)
        {
        }

        public PadTriggerHandler(string name, PlayerIndex playerIndex, Trigger trigger, float deadZone = 0,
            AxisSign sign = AxisSign.None, bool inverse = false, InputActivity desiredActivity = InputActivity.Pressed)
            : base(name, deadZone, sign, inverse, desiredActivity)
        {
            PlayerIndex = playerIndex;
            Trigger = trigger;
        }

        protected override float GetState(InputStates inputStates)
        {
            switch (Trigger)
            {
                case Trigger.None:
                    return 0;
                case Trigger.Left:
                    return inputStates.GamePadStates[PlayerIndex].Triggers.Left;
                case Trigger.Right:
                    return inputStates.GamePadStates[PlayerIndex].Triggers.Right;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}