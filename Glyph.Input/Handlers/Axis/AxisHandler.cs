using Glyph.Input.Behaviours;

namespace Glyph.Input.Handlers.Axis
{
    public abstract class AxisHandler : SensitiveHandler<float>
    {
        public float DeadZone { get; set; }
        public AxisSign Sign { get; set; }
        public override float Value { get; protected set; }

        protected AxisHandler(string name, float deadZone, AxisSign sign, InputActivity desiredActivity)
            : base(name, desiredActivity)
        {
            DeadZone = deadZone;
            Sign = sign;
        }

        protected override void HandleInput(InputStates inputStates)
        {
            float state = GetState(inputStates);

            IsActivated = (Sign == AxisSign.Positive || Sign == AxisSign.None) && state >= DeadZone
                           || (Sign == AxisSign.Negative || Sign == AxisSign.None) && state <= -DeadZone;

            Value = !IsActivated ? state : 0;
        }

        protected abstract float GetState(InputStates inputStates);
    }
}