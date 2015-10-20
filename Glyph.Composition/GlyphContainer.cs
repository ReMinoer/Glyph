using Diese.Composition;

namespace Glyph.Composition
{
    public abstract class GlyphContainer : GlyphContainer<IGlyphComponent>
    {
        protected GlyphContainer(int size)
            : base(size)
        {
        }
    }

    public abstract class GlyphContainer<TComponent> : Container<IGlyphComponent, IGlyphParent, TComponent>, IGlyphContainer<TComponent>
        where TComponent : class, IGlyphComponent
    {
        public virtual bool IsStatic { get; protected set; }

        protected GlyphContainer(int size)
            : base(size)
        {
        }

        public virtual void Initialize()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}