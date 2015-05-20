using System;
using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors.Cursors
{
    public abstract class CursorHandler : SurfaceHandler
    {
        public CursorSpace CursorSpace { get; protected set; }

        protected CursorHandler(string name, CursorSpace cursorSpace) : base(name)
        {
            CursorSpace = cursorSpace;
        }

        protected override Vector2 GetState(InputManager inputManager)
        {
            Vector2 state = GetCursor(inputManager);

            var cursorInWindow = new Vector2(state.X, state.Y);

            switch (CursorSpace)
            {
                case CursorSpace.Window:
                    return cursorInWindow;
                case CursorSpace.Screen:
                    return cursorInWindow - Resolution.WindowMargin;
                case CursorSpace.VirtualScreen:
                    return (cursorInWindow - Resolution.WindowMargin) / Resolution.ScaleRatio;
                case CursorSpace.Scene:
                    return (cursorInWindow - Resolution.WindowMargin) / (Resolution.ScaleRatio * Camera.Zoom)
                           + Camera.VectorPosition.XY();
                default:
                    throw new NotImplementedException();
            }
        }

        protected abstract Vector2 GetCursor(InputManager inputManager);
    }
}