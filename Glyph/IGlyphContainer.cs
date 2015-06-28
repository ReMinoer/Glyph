using Diese.Composition;

namespace Glyph
{
    public interface IGlyphContainer : IGlyphEnumerable<IGlyphComponent>, IContainer<IGlyphComponent, IGlyphParent>
    {
    }
}