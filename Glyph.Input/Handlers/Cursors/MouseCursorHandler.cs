using Microsoft.Xna.Framework;

namespace Glyph.Input.Handlers.Cursors
{
    public class MouseCursorHandler : CursorHandler
    {
        public override InputSource InputSource
        {
            get { return InputSource.Mouse; }
        }

        public MouseCursorHandler()
            : this("", CursorSpace.Window)
        {
        }

        public MouseCursorHandler(string name, CursorSpace cursorSpace)
            : base(name, cursorSpace)
        {
        }

        protected override Vector2 GetState(InputStates inputStates)
        {
            return inputStates.MouseState.Position.ToVector2();
        }
    }
}