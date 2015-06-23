using System;
using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComponent : IComponent<IGlyphComponent, GlyphEntity>, IDisposable
    {
        void Initialize();
    }
}