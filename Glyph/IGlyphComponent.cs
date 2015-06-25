using System;
using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComponent : IComponent<IGlyphComponent, GlyphObject>, IDisposable
    {
        void Initialize();
    }
}