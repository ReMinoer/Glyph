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

        public PadTriggerHandler(string name, Trigger trigger, PlayerIndex playerIndex = PlayerIndex.One)
            : base(name)
        {
            PlayerIndex = playerIndex;
            Trigger = trigger;
        }

        protected override float GetState(InputManager inputManager)
        {
            return Trigger == Trigger.Left
                ? inputManager.GamePadStates[PlayerIndex].Triggers.Left
                : inputManager.GamePadStates[PlayerIndex].Triggers.Right;
        }
    }
}