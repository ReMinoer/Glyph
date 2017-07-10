using Diese.Injection;

namespace Glyph.Injection
{
    public class GlyphInjectableCollectionAttribute : InjectableCollectionAttribute, IGlyphInjectableAttribute
    {
        public GlyphInjectableTargets Targets { get; }

        public GlyphInjectableCollectionAttribute()
        {
            Targets = GlyphInjectableTargets.All;
        }

        public GlyphInjectableCollectionAttribute(GlyphInjectableTargets targets)
        {
            Targets = targets;
        }
    }
}