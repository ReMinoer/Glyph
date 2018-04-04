using Niddle;

namespace Glyph.Injection
{
    public class GlyphInjectableAttribute : InjectableAttribute, IGlyphInjectableAttribute
    {
        public GlyphInjectableTargets Targets { get; }

        public GlyphInjectableAttribute()
        {
            Targets = GlyphInjectableTargets.All;
        }

        public GlyphInjectableAttribute(GlyphInjectableTargets targets)
        {
            Targets = targets;
        }
    }
}