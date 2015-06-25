using Diese.Composition;

namespace Glyph
{
    public interface IGlyphComposite : IGlyphComponent, IComposite<IGlyphComponent, GlyphObject>
    {
    }
}