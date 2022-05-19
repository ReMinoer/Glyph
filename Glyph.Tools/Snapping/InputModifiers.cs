using Fingear.Controls;
using Fingear.Inputs;
using Fingear.MonoGame;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Inputs;
using Microsoft.Xna.Framework.Input;

namespace Glyph.Tools.Snapping
{
    public class InputModifiers : GlyphObject
    {
        private readonly Control _ctrl;
        private readonly Control _alt;
        public bool CtrlPressed { get; private set; }
        public bool AltPressed { get; private set; }

        public InputModifiers(GlyphResolveContext context, IGlyphComponent root)
            : base(context)
        {
            var controls = Add<Controls>();
            controls.Interactive.Controls.Add(_ctrl = new Control("Ctrl", InputSystem.Instance.Keyboard[Keys.LeftControl], InputActivity.Pressed));
            controls.Interactive.Controls.Add(_alt = new Control("Alt", InputSystem.Instance.Keyboard[Keys.LeftAlt], InputActivity.Pressed));

            Schedulers.Update.Plan(HandleInput);
        }

        private void HandleInput(ElapsedTime elapsedTime)
        {
            CtrlPressed = _ctrl.IsActive;
            AltPressed = _alt.IsActive;
        }
    }
}