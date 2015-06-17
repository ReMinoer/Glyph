using System;
using System.Linq.Expressions;
using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComponent : IComponent<IGlyphComponent>, IDisposable
    {
        void Initialize();
    }
}