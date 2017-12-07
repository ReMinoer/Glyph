using Diese.Injection;
using Diese.Modelization;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphCreator<out T> : ICreator<T>
    {
        IDependencyInjector Injector { set; }
    }
}