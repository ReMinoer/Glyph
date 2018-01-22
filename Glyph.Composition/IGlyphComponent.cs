using System;
using Stave;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphContainer>, IDisposable
    {
        Guid Id { get; }
        string Name { get; set; }
        bool Disposed { get; }
        void Initialize();
    }
}