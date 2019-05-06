using Diese.Collections;
using Glyph.Resolver;
using Simulacra;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphConfigurator : IDataBindable<IGlyphComponent>, IResolveClient
    {
        IReadOnlyObservableCollection<IGlyphCreator> Children { get; }
    }

    public interface IGlyphConfigurator<in T> : IGlyphConfigurator, ICompositeConfigurator<T, IGlyphConfigurator<T>>
        where T : IGlyphComponent
    {
    }
}