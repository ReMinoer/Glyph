using Diese.Collections.Observables.ReadOnly;
using Simulacra;
using Simulacra.Injection;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphData : IBindableData, IDependencyResolverClient
    {
        string Name { get; }
        new IGlyphComponent BindedObject { get; }
        IReadOnlyObservableCollection<IGlyphData> Children { get; }
    }
}