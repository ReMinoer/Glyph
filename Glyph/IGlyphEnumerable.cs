using Diese.Composition;

namespace Glyph
{
    public interface IGlyphEnumerable<out TInput> : IGlyphParent, IComponentEnumerable<IGlyphComponent, IGlyphParent, TInput>
        where TInput : IGlyphComponent
    {
    }
}