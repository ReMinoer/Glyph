using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public abstract class VectorHandler : InputHandler<Vector2>
    {
        public override bool IsActivated { get; protected set; }
        public override Vector2 Value { get; protected set; }
        public float DeadZone { get; private set; }

        protected VectorHandler(string name, float deadZone = 0) : base(name)
        {
            DeadZone = deadZone;
        }

        public override void Update(InputStates inputStates)
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