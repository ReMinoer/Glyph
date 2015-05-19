namespace Glyph.Input.Handlers.Axis
{
    public abstract class AxisHandler : InputHandler<float>
    {
        public override bool IsActivated { get; protected set; }
        public override float Value { get; protected set; }
        public float DeadZone { get; private set; }

        public AxisHandler(string name)
            : base(name)
        {
            DeadZone = 0;
        }

        public override void Update(InputManager inputManager)
        {
            float state = GetState(inputManager);
            if (state < DeadZone)
                state = 0;

            Value = state;
            IsActivated = Value >= DeadZone;
        }

        protected abstract float GetState(InputManager inputManager);
    }
}