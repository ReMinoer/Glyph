using System.Numerics;
using Fingear;
using Fingear.Controls;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;

namespace Glyph.Core.Inputs
{
    public class CursorControls : ControlLayerBase
    {
        public IControl<Vector2> WindowPosition { get; }
        public IControl<Vector2> ScreenPosition { get; }
        public IControl<Vector2> VirtualScreenPosition { get; }
        public IControl<Vector2> ScenePosition { get; }
        public IControl Clic { get; }
        public IControl ReleaseClic { get; }

        public CursorControls(InputClientManager inputClientManager)
        {
            MouseSource mouse = InputSystem.Instance.Mouse;

            Add(WindowPosition = new ReferentialCursorControl(inputClientManager, "WindowPosition", mouse.Cursor, CursorSpace.Window));
            Add(ScreenPosition = new ReferentialCursorControl(inputClientManager, "ScreenPosition", mouse.Cursor, CursorSpace.Screen));
            Add(VirtualScreenPosition = new ReferentialCursorControl(inputClientManager, "VirtualScreenPosition", mouse.Cursor, CursorSpace.VirtualScreen));
            Add(ScenePosition = new ReferentialCursorControl(inputClientManager, "ScenePosition", mouse.Cursor, CursorSpace.Scene));
            Add(Clic = new Control(mouse[MouseButton.Left]));
            Add(ReleaseClic = new Control(mouse[MouseButton.Left], InputActivity.Released));
        }
    }
}