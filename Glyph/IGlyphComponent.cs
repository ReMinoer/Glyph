using System;
using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        void Initialize();
    }
}