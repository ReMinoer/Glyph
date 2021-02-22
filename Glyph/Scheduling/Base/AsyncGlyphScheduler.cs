using System;
using System.Threading;
using System.Threading.Tasks;
using Taskete.Schedulers;

namespace Glyph.Scheduling.Base
{
    public class AsyncGlyphScheduler<T, TDelegate, TParam> : GlyphSchedulerBase<T, TDelegate>
    {
        private readonly AsyncScheduler<T, TParam> _scheduler;

        public AsyncGlyphScheduler(Func<TDelegate, T> delegateToTaskFunc, Func<T, TParam, CancellationToken, Task> awaitableSelector)
            : base(new AsyncScheduler<T, TParam>(awaitableSelector), delegateToTaskFunc)
        {
            _scheduler = (AsyncScheduler<T, TParam>)Scheduler;
        }

        public Task RunScheduleAsync(TParam param) => _scheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _scheduler.RunScheduleAsync(param, cancellationToken);
    }
}