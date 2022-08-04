using Fingear.Interactives.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace Glyph.UI
{
    public interface IGlyphInteractiveInterface : IInteractiveInterface<IGlyphInteractiveInterface>
    {
        MouseCursor HoverCursor { get; }
        MouseCursor TouchCursor { get; }
    }
}