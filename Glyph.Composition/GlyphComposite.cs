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
        }
    }
}