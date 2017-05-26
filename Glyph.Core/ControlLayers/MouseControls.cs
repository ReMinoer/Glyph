using System.Numerics;
using Fingear;
using Fingear.Controls;
using Fingear.Converters;
using Fingear.MonoGame;
using Fingear.MonoGame.Inputs;
using Glyph.Core.Controls;
using Glyph.Input;

namespace Glyph.Core.ControlLayers
{
    public class MouseControls : ControlLayerBase
    {
        public IControl<Vector2> WindowPosition { get; }
        public IControl<Vector2> ScreenPosition { get; }
        public IControl<Vector2> VirtualScreenPosition { get; }
        public IControl<Vector2> ScenePosition { get; }
        public IControl<InputActivity> Left { get; }
        public IControl<InputActivity> Right { get; }
        public IControl<InputActivity> Middle { get; }
        public IControl<float> Wheel { get; }

        public MouseControls(ControlManager controlManager)
        {
            MouseSource mouse = MonoGameInputSytem.Instance.Mouse;

            Add(WindowPosition = new ReferentialCursorControl(controlManager, "WindowPosition", mouse.Cursor, CursorSpace.Window));
            Add(ScreenPosition = new ReferentialCursorControl(controlManager, "ScreenPosition", mouse.Cursor, CursorSpace.Screen));
            Add(VirtualScreenPosition = new ReferentialCursorControl(controlManager, "VirtualScreenPosition", mouse.Cursor, CursorSpace.VirtualScreen));
            Add(ScenePosition = new ReferentialCursorControl(controlManager, "ScenePosition", mouse.Cursor, CursorSpace.Scene));
            Add(Left = new ActivityControl("Left", mouse[MouseButton.Left]));
            Add(Right = new ActivityControl("Right", mouse[MouseButton.Right]));
            Add(Middle = new ActivityControl("Middle", mouse[MouseButton.Middle]));
            Add(Wheel = new Control<float>("Wheel", mouse.Wheel.Force()));
        }
    }
}