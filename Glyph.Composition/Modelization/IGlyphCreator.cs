using Simulacra;

namespace Glyph.Composition.Modelization
{
    public interface IGlyphCreator : IGlyphData, IInstantiatingData
    {
    }

    public interface IGlyphCreator<out T> : IGlyphCreator, ICreator<T>, IInstantiatingData<T>
        where T : IGlyphComponent
    {
        new T BindedObject { get; }
    }
}