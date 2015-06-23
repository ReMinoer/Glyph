using System;

namespace Glyph
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; }
        event EventHandler VisibleChanged;
        void Draw();
    }
}