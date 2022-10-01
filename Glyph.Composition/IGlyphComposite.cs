using Stave;
using System.Collections.Generic;

namespace Glyph.Composition
{
    public interface IGlyphComposite : IGlyphComposite<IGlyphComponent>
    {
    }

    public interface IGlyphComposite<TComponent> : IOrderedComposite<IGlyphComponent, IGlyphContainer, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        void RemoveAndDispose(TComponent item);
        void ClearAndDisposeComponents();

        T GetKeyedComponent<T>(object key) where T : class, TComponent;
        bool SetKeyedComponent(object key, TComponent component);

        IList<TComponent> CreateSubComposite(object key);
        bool RemoveSubComposite(object key);
        IList<TComponent> GetSubComposite(object key);
    }
}