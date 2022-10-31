using System;
using Niddle;

namespace Glyph.Composition
{
    public interface IGlyphCompositeResolver : IGlyphCompositeResolver<IGlyphComponent>
    {
        IDependencyResolver DependencyResolver { get; }
    }

    public interface IGlyphCompositeResolver<TComponent> : IGlyphComposite<TComponent>
        where TComponent : class, IGlyphComponent
    {
        T Add<T>() where T : IGlyphComponent;
        T Add<T>(Action<T> beforeAdding) where T : IGlyphComponent;
    }
}