﻿using Fingear;
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
            Add(WindowPosition = new ReferentialCursorControl("WindowPosition", new MouseCursorInput(), CursorSpace.Window));
            Add(ScreenPosition = new ReferentialCursorControl("ScreenPosition", new MouseCursorInput(), CursorSpace.Screen));
            Add(VirtualScreenPosition = new ReferentialCursorControl("VirtualScreenPosition", new MouseCursorInput(), CursorSpace.VirtualScreen));
            Add(ScenePosition = new ReferentialCursorControl("ScenePosition", new MouseCursorInput(), CursorSpace.Scene));
            Add(Left = new ActivityControl("Left", new MouseButtonInput(MouseButton.Left)));
            Add(Right = new ActivityControl("Right", new MouseButtonInput(MouseButton.Right)));
            Add(Middle = new ActivityControl("Middle", new MouseButtonInput(MouseButton.Middle)));
            Add(Wheel = new Control<float>("Wheel", new MouseWheelInput()));
        }
    }
}