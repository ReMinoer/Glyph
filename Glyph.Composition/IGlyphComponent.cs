using System;
using Glyph.Composition.Messaging;
using Stave;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphContainer>, IDisposable
    {
        bool Enabled { get; set; }
        bool Active { get; }
        Guid Id { get; }
        string Name { get; set; }
        ComponentRouterSystem Router { get; }
        bool Disposed { get; }
        void Initialize();
    }
}