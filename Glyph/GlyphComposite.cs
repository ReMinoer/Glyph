using Diese.Composition;

namespace Glyph
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