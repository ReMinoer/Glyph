using Stave;

namespace Glyph.Composition
{
    public interface IGlyphContainer : IGlyphContainer<IGlyphComponent>
    {
    }

    public interface IGlyphContainer<TComponent> : IContainer<IGlyphComponent, IGlyphParent, TComponent>, IGlyphParent<TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}