using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphContainer : IGlyphEnumerable<IGlyphComponent>, IContainer<IGlyphComponent, IGlyphParent>
    {
    }
}