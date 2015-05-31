using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public abstract class VectorHandler : SensitiveHandler<Vector2>
    {
        public float DeadZone { get; set; }
        public bool InverseX { get; set; }
        public bool InverseY { get; set; }
        public override Vector2 Value { get; protected set; }

        protected VectorHandler(string name, float deadZone, bool inverseX, bool inverseY, InputActivity desiredActivity)
            : base(name, desiredActivity)
        {
            DeadZone = deadZone;
            InverseX = inverseX;
            InverseY = inverseY;
        }

        protected abstract Vector2 GetState(InputStates inputStates);

        protected override void HandleInput(InputStates inputStates)
        {
            Vector2 state = GetState(inputStates);
            if (state.Length() < DeadZone)
                state = Vector2.Zero;

            state = state.Multiply(InverseX ? -1 : 1, InverseY ? -1 : 1);

            Value = state;
            IsActivated = Value.Length() >= DeadZone;
        }
    }
}