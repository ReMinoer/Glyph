using Diese.Collections;
using Diese.Modelization;
using Glyph.Injection;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphConfigurator : IDataBindable<IGlyphComponent>, IInjectionClient
    {
        IReadOnlyObservableCollection<IGlyphCreator> Children { get; }
    }

    public interface IGlyphConfigurator<in T> : IGlyphConfigurator, ICompositeConfigurator<T, IGlyphConfigurator<T>>
        where T : IGlyphComponent
    {
    }
}