using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphContainer : IGlyphContainer<IGlyphComponent>
    {
    }

    public interface IGlyphContainer<out TComponent> : IContainer<IGlyphComponent, IGlyphParent, TComponent>, IGlyphParent
        where TComponent : class, IGlyphComponent
    {
    }
}