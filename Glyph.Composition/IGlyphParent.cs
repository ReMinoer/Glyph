using Stave;

namespace Glyph.Composition
{
    public interface IGlyphParent : IGlyphComponent, IParent<IGlyphComponent, IGlyphParent>
    {
        bool IsFreeze { get; set; }
    }

    public interface IGlyphParent<TComponent> : IGlyphParent, IParent<IGlyphComponent, IGlyphParent, TComponent>
        where TComponent : class, IGlyphComponent
    {
    }
}