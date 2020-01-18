using System;

namespace Glyph.Composition
{
    public interface IGlyphCompositeResolver : IGlyphCompositeResolver<IGlyphComponent>
    {

    }
    public interface IGlyphCompositeResolver<TComponent> : IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        T Add<T>() where T : IGlyphComponent;
        T Add<T>(Action<T> beforeAdding) where T : IGlyphComponent;
    }
}