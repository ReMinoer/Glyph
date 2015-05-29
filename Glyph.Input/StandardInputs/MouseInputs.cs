using System.Collections.Generic;
using Glyph.Input.Handlers.Axis;
using Glyph.Input.Handlers.Buttons;
using Glyph.Input.Handlers.Cursors;

namespace Glyph.Input.StandardInputs
{
    public class MouseInputs : List<IInputHandler>
    {
        private const string Prefix = "Mouse-";
        public const string WindowPosition = Prefix + "WindowPosition";
        public const string ScreenPosition = Prefix + "ScreenPosition";
        public const string VirtualScreenPosition = Prefix + "VirtualScreenPosition";
        public const string ScenePosition = Prefix + "ScenePosition";
        public const string Left = Prefix + "Left";
        public const string LeftReleased = Prefix + "LeftReleased";
        public const string LeftPressed = Prefix + "LeftPressed";
        public const string Right = Prefix + "Right";
        public const string RightReleased = Prefix + "RightReleased";
        public const string RightPressed = Prefix + "RightPressed";
        public const string Middle = Prefix + "Middle";
        public const string MiddleReleased = Prefix + "MiddleReleased";
        public const string MiddlePressed = Prefix + "MiddlePressed";
        public const string Wheel = Prefix + "Wheel";

        public MouseInputs()
        {
            AddRange(new List<IInputHandler>
            {
                new MouseCursorHandler(WindowPosition, CursorSpace.Window),
                new MouseCursorHandler(ScreenPosition, CursorSpace.Screen),
                new MouseCursorHandler(VirtualScreenPosition, CursorSpace.VirtualScreen),
                new MouseCursorHandler(ScenePosition, CursorSpace.Scene),

                new MouseButtonHandler(Left, MouseButton.Left),
                new MouseButtonHandler(LeftReleased, MouseButton.Left, InputActivity.Released),
                new MouseButtonHandler(LeftPressed, MouseButton.Left, InputActivity.Pressed),
                new MouseButtonHandler(Right, MouseButton.Right),
                new MouseButtonHandler(RightReleased, MouseButton.Right, InputActivity.Released),
                new MouseButtonHandler(RightPressed, MouseButton.Right, InputActivity.Pressed),
                new MouseButtonHandler(Middle, MouseButton.Middle),
                new MouseButtonHandler(MiddleReleased, MouseButton.Middle, InputActivity.Released),
                new MouseButtonHandler(MiddlePressed, MouseButton.Middle, InputActivity.Pressed),

                new MouseWheelHandler(Wheel)
            });
        }
    }
}