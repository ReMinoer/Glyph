using System;
using Stave;

namespace Glyph.Composition
{
    // TODO : Add instantiating & disposing router as injectable properties
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        string Name { get; set; }
        void Initialize();
    }
}