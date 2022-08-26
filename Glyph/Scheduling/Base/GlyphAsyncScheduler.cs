using System;
using System.Threading;
using System.Threading.Tasks;
using Taskete.Schedulers;

namespace Glyph.Scheduling.Base
{
    public class GlyphAsyncScheduler<T, TAsyncDelegate, TDelegate, TParam> : GlyphSchedulerBase<T, TDelegate>,
        IGlyphAsyncDelegateScheduler<T, TAsyncDelegate, TDelegate, GlyphSchedulerBase<T, TDelegate>.Controller, GlyphSchedulerBase<T, TDelegate>.Controller>
        where T : class
    {
        private readonly Func<TAsyncDelegate, TDelegate, T> _delegateToAsyncTaskFunc;
        private readonly AsyncScheduler<T, TParam> _scheduler;

        public GlyphAsyncScheduler(Func<TAsyncDelegate, TDelegate, T> delegateToAsyncTaskFunc,
            Func<TDelegate, T> delegateToTaskFunc,
            Func<T, TParam, CancellationToken, Task> awaitableSelector,
            Action<T, TParam, CancellationToken> actionInvoker)
            : base(new AsyncScheduler<T, TParam>(awaitableSelector, actionInvoker), delegateToTaskFunc)
        {
            _delegateToAsyncTaskFunc = delegateToAsyncTaskFunc;
            _scheduler = (AsyncScheduler<T, TParam>)Scheduler;
        }

        public void RunSchedule(TParam param) => _scheduler.RunSchedule(param);
        public void RunSchedule(TParam param, CancellationToken cancellationToken) => _scheduler.RunSchedule(param, cancellationToken);
        public Task RunScheduleAsync(TParam param) => _scheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _scheduler.RunScheduleAsync(param, cancellationToken);

        public Controller Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate) => Plan(GetOrAddAsyncDelegateTask(asyncTaskDelegate, taskDelegate));
        void IGlyphAsyncDelegateScheduler<T, TAsyncDelegate, TDelegate>.Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate) => Plan(asyncTaskDelegate, taskDelegate);

        private T GetOrAddAsyncDelegateTask(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate)
            => GetOrAddTask(taskDelegate, () => _delegateToAsyncTaskFunc(asyncTaskDelegate, taskDelegate));
    }
}