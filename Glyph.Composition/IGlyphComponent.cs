using System;
using Diese.Composition;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        void Initialize();
    }
}