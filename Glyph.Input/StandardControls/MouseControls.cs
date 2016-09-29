using Fingear;
using Fingear.Controls;
using Fingear.MonoGame.Inputs;
using Glyph.Input.Controls;

namespace Glyph.Input.StandardControls
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

        public MouseControls()
        {
            WindowPosition = new ReferentialCursorControl("WindowPosition", new MouseCursorInput(), CursorSpace.Window);
            ScreenPosition = new ReferentialCursorControl("ScreenPosition", new MouseCursorInput(), CursorSpace.Screen);
            VirtualScreenPosition = new ReferentialCursorControl("VirtualScreenPosition", new MouseCursorInput(), CursorSpace.VirtualScreen);
            ScenePosition = new ReferentialCursorControl("ScenePosition", new MouseCursorInput(), CursorSpace.Scene);
            Left = new ActivityControl("Left", new MouseButtonInput(MouseButton.Left));
            Right = new ActivityControl("Right", new MouseButtonInput(MouseButton.Right));
            Middle = new ActivityControl("Middle", new MouseButtonInput(MouseButton.Middle));
            Wheel = new Control<float>("Wheel", new MouseWheelInput());
        }
    }
}