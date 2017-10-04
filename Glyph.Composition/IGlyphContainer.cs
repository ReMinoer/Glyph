using Stave;

namespace Glyph.Composition
{
    public interface IGlyphContainer : IGlyphComponent, IContainer<IGlyphComponent, IGlyphContainer>
    {
        bool IsFreeze { get; set; }
    }

    public interface IGlyphContainer<TComponent> : IGlyphContainer, IContainer<IGlyphComponent, IGlyphContainer, TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}