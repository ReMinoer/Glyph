using Microsoft.Xna.Framework.Input;

namespace Glyph.Core.Inputs
{
    public class CursorManager
    {
        private MouseCursor _cursor;
        public MouseCursor Cursor
        {
            get => _cursor;
            set
            {
                if (_cursor == value)
                    return;

                _cursor = value;
                ChangeRequested = true;
            }
        }

        public bool ChangeRequested { get; private set; }
        public void ResetRequest() => ChangeRequested = false;
    }
}