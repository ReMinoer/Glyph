using Simulacra;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphConfigurator<T> : IGlyphData, IBindableData<T>, IConfigurator<T>
        where T : IGlyphComponent
    {
        new T BindedObject { get; }
    }
}