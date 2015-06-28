using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComposite : IGlyphEnumerable<IGlyphComponent>, IComposite<IGlyphComponent, IGlyphParent>
    {
    }
}