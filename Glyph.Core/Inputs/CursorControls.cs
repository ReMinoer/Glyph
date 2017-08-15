using Fingear.MonoGame;

namespace Glyph.Core.Inputs
{
    public class CursorControls
    {
        private readonly InputClientManager _inputClientManager;
        private readonly MouseSource _mouseSource;
        public ReferentialCursorControl WindowPosition => new ReferentialCursorControl(_inputClientManager, nameof(WindowPosition), _mouseSource.Cursor, CursorSpace.Window);
        public ReferentialCursorControl ScreenPosition => new ReferentialCursorControl(_inputClientManager, nameof(ScreenPosition), _mouseSource.Cursor, CursorSpace.Screen);
        public ReferentialCursorControl VirtualScreenPosition => new ReferentialCursorControl(_inputClientManager, nameof(VirtualScreenPosition), _mouseSource.Cursor, CursorSpace.VirtualScreen);
        public ReferentialCursorControl ScenePosition => new ReferentialCursorControl(_inputClientManager, nameof(ScenePosition), _mouseSource.Cursor, CursorSpace.Scene);

        public CursorControls(InputClientManager inputClientManager)
        {
            _inputClientManager = inputClientManager;
            _mouseSource = InputSystem.Instance.Mouse;
        }
    }
}