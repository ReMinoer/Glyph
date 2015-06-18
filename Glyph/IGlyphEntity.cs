using System;

namespace Glyph
{
    public interface IGlyphEntity : IGlyphComposite, ILoadContent, IHandleInput, IDraw
    {
        event EventHandler UpdateOrderChanged;
        event EventHandler DrawOrderChanged;
    }
}