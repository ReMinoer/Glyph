using System.Collections.Generic;
using Diese.Modelization;
using Glyph.Injection;

namespace Glyph.Modelization
{
    public interface IGlyphCreator<out T> : ICreator<T>, IInjectionClient
    {
        IEnumerable<IInjectionClient> DataChildren { get; }
    }
}