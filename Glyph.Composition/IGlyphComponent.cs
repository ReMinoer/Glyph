using System;
using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        void Initialize();
    }
}