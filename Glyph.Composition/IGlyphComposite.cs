using Stave;

namespace Glyph.Composition
{
    public interface IGlyphComposite : IGlyphComposite<IGlyphComponent>
    {
    }

    public interface IGlyphComposite<TComponent> : IComposite<IGlyphComponent, IGlyphParent, TComponent>, IGlyphParent<TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}