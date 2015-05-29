using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public abstract class VectorHandler : SensitiveHandler<Vector2>
    {
        public float DeadZone { get; set; }
        public override Vector2 Value { get; protected set; }

        protected VectorHandler(string name, float deadZone, InputActivity desiredActivity)
            : base(name, desiredActivity)
        {
            DeadZone = deadZone;
        }

        protected override void HandleInput(InputStates inputStates)
        {
            Vector2 state = GetState(inputStates);
            if (state.Length() < DeadZone)
                state = Vector2.Zero;

            Value = state;
            IsActivated = Value.Length() >= DeadZone;
        }

        protected abstract Vector2 GetState(InputStates inputStates);
    }
}