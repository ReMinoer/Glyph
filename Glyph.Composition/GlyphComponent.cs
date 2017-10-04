using Glyph.Composition.Base;
using Stave;

namespace Glyph.Composition
{
    public class GlyphComponent : GlyphComponentBase
    {
        internal override sealed IComponent<IGlyphComponent, IGlyphContainer> ComponentImplementation { get; }

        public GlyphComponent()
        {
            ComponentImplementation = new Component<IGlyphComponent, IGlyphContainer>(this);
        }
    }
}