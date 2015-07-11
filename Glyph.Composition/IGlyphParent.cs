using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphParent : IGlyphComponent, IParent<IGlyphComponent, IGlyphParent>
    {
    }
}