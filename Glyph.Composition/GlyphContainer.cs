using Diese.Composition;

namespace Glyph.Composition
{
    public abstract class GlyphContainer : Container<IGlyphComponent, IGlyphParent, IGlyphComponent>, IGlyphContainer
    {
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