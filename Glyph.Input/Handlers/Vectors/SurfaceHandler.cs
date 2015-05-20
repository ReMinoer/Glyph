using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors
{
    public abstract class SurfaceHandler : InputHandler<Vector2>
    {
        private Vector2 _previousState;
        public override bool IsActivated { get; protected set; }
        public override Vector2 Value { get; protected set; }

        protected SurfaceHandler(string name)
            : base(name)
        {
        }

        public override void Update(InputManager inputManager)
        {
            Vector2 state = GetState(inputManager);

            Value = state;
            IsActivated = state != _previousState;

            _previousState = state;
        }

        protected abstract Vector2 GetState(InputManager inputManager);
    }
}