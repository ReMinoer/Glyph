using Fingear;
using Fingear.Controls;
using Fingear.MonoGame.Inputs;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Input.StandardControls
{
    public class EditorControls : ControlLayerBase
    {
        public IControl EditorMode { get; }
        public IControl ShiftPressed { get; }

        public EditorControls()
        {
            Add(EditorMode = new Control("EditorMode", new KeyInput(Keys.F1)));
            Add(ShiftPressed = new Control("ShiftPressed", new KeyInput(Keys.LeftShift), InputActivity.Pressed));
        }
    }
}