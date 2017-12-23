using System.Collections.Generic;
using Diese.Modelization;
using Glyph.Injection;

namespace Glyph.Modelization
{
    public interface IBindedGlyphCreator : IInjectionClient, IDataBindable
    {
        void Instantiate();
        IEnumerable<IBindedGlyphCreator> DataChildren { get; }
    }
    
    public interface IBindedGlyphCreator<out T> : IBindedGlyphCreator, IGlyphCreator<T>, IDataBindable<T>
    {
    }
}