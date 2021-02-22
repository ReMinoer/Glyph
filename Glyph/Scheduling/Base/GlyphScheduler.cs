using System;
using System.Collections.Generic;
using Taskete.Schedulers;

namespace Glyph.Scheduling.Base
{
    public class GlyphScheduler<T, TDelegate> : GlyphSchedulerBase<T, TDelegate>
    {
        private readonly LinearScheduler<T> _scheduler;
        public IEnumerable<T> Schedule => _scheduler.Schedule;

        public GlyphScheduler(Func<TDelegate, T> delegateToTaskFunc)
            : base(new LinearScheduler<T>(), delegateToTaskFunc)
        {
            _scheduler = (LinearScheduler<T>)Scheduler;
        }
    }
}