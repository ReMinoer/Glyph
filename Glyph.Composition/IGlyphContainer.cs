using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphContainer : IGlyphContainer<IGlyphComponent>
    {
    }

    public interface IGlyphContainer<out TComponent> : IContainer<IGlyphComponent, IGlyphParent, TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}