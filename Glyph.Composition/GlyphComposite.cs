using System;
using Diese.Composition;

namespace Glyph.Composition
{
    public abstract class GlyphComposite : Composite<IGlyphComponent, IGlyphParent>, IGlyphComposite
    {
        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
            foreach (IGlyphComponent component in Components)
                component.Dispose();

            Clear();
        }

        public abstract T Add<T>() where T : class, IGlyphComponent, new();
        public abstract IGlyphComponent Add(Type componentType);
    }
}