using System;
using Diese.Collections;
using Niddle.Base;

namespace Glyph.Injection
{
    public class GlyphInjectableTrackerAttribute : InjectableManyByGenericMethodAttributeBase, IGlyphInjectableAttribute
    {
        public GlyphInjectableTargets Targets { get; }

        protected override Type GenericTypeDefinition => typeof(ITracker<>);
        protected override string MethodName => nameof(ITracker<object>.Register);

        public GlyphInjectableTrackerAttribute()
        {
            Targets = GlyphInjectableTargets.All;
        }

        public GlyphInjectableTrackerAttribute(GlyphInjectableTargets targets)
        {
            Targets = targets;
        }
    }
}