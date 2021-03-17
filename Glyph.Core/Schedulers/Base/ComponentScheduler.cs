using System.Collections.Generic;
using Glyph.Scheduling.Base;

namespace Glyph.Core.Schedulers.Base
{
    public class ComponentScheduler<T, TDelegate> : ComponentSchedulerBase<T, TDelegate>
        where T : class
    {
        private readonly GlyphScheduler<T, TDelegate> _glyphScheduler;
        public IEnumerable<T> Schedule => _glyphScheduler.Schedule;

        public ComponentScheduler(GlyphScheduler<T, TDelegate> glyphScheduler)
            : base(glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;
        }
    }
}