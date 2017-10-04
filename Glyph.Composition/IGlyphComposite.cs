using Stave;

namespace Glyph.Composition
{
    public interface IGlyphComposite : IGlyphComposite<IGlyphComponent>
    {
    }

    public interface IGlyphComposite<TComponent> : IComposite<IGlyphComponent, IGlyphContainer, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}