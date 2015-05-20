namespace Glyph.Input.Handlers.Axis
{
    public abstract class AxisHandler : InputHandler<float>
    {
        public override bool IsActivated { get; protected set; }
        public override float Value { get; protected set; }
        public float DeadZone { get; private set; }

        public AxisHandler(string name, float deadZone = 0)
            : base(name)
        {
            DeadZone = deadZone;
        }

        public override void Update(InputStates inputStates)
        {
            float state = GetState(inputStates);
            if (state < DeadZone)
                state = 0;

            Value = state;
            IsActivated = Value >= DeadZone;
        }

        protected abstract float GetState(InputStates inputStates);
    }
}