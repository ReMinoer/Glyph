using Diese.Composition;

namespace Glyph
{
    public interface IGlyphParent : IGlyphComponent, IParent<IGlyphComponent, IGlyphParent>
    {
    }
}