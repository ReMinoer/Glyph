using System;
using System.Linq;
using Glyph.Core;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Cursors
{
    public abstract class CursorHandler : SensitiveHandler<Vector2>
    {
        private Vector2 _previousState;
        public CursorSpace CursorSpace { get; set; }
        public override Vector2 Value { get; protected set; }

        protected CursorHandler(string name, CursorSpace cursorSpace, InputActivity desiredActivity)
            : base(name, desiredActivity)
        {
            CursorSpace = cursorSpace;
        }

        protected abstract Vector2 GetState(InputStates inputStates);

        protected override void HandleInput(InputStates inputStates)
        {
            Vector2 state = GetState(inputStates);

            Vector2 position;
            switch (CursorSpace)
            {
                case CursorSpace.Window:
                    position = state;
                    break;
                case CursorSpace.Screen:
                    position = state - Resolution.Instance.WindowMargin;
                    break;
                case CursorSpace.VirtualScreen:
                    position = (state - Resolution.Instance.WindowMargin) / Resolution.Instance.ScaleRatio;
                    break;
                case CursorSpace.Scene:
                    IView view = ViewManager.Main.GetViewAtWindowPoint(state.ToPoint()) ?? ViewManager.Main.Views.First();

                    position = (state - Resolution.Instance.WindowMargin) /
                               (Resolution.Instance.ScaleRatio * view.Camera.Zoom)
                               + view.Camera.Position;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            Value = position;
            IsActivated = state != _previousState;

            _previousState = state;
        }
    }
}