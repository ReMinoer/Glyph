using Diese.Composition;

namespace Glyph.Composition
{
    public interface IGlyphEnumerable<out TInput> : IGlyphParent,
        IComponentEnumerable<IGlyphComponent, IGlyphParent, TInput>
        where TInput : IGlyphComponent
    {
    }
}