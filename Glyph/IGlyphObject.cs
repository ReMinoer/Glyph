using System;

namespace Glyph
{
    public interface IGlyphObject : IGlyphComposite, ILoadContent, IHandleInput, IDraw
    {
        event EventHandler UpdateOrderChanged;
        event EventHandler DrawOrderChanged;
    }
}