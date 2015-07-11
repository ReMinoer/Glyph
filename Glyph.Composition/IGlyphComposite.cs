using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphComposite : IGlyphEnumerable<IGlyphComponent>, IComposite<IGlyphComponent, IGlyphParent>
    {
    }
}