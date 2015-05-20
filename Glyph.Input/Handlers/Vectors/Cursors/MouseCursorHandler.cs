using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Vectors.Cursors
{
    public class MouseCursorHandler : CursorHandler
    {
        public override InputSource InputSource
        {
            get { return InputSource.Mouse; }
        }

        public MouseCursorHandler(string name, CursorSpace cursorSpace)
            : base(name, cursorSpace)
        {
        }

        protected override Vector2 GetCursor(InputManager inputManager)
        {
            return inputManager.MouseState.Position.ToVector2();
        }
    }
}