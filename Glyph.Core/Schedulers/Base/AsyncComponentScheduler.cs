using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Glyph.Scheduling;
using Glyph.Scheduling.Base;

namespace Glyph.Core.Schedulers.Base
{
    public class AsyncComponentScheduler<T, TAsyncDelegate, TDelegate, TParam> : ComponentSchedulerBase<T, TDelegate>,
        IGlyphAsyncDelegateScheduler<T, TAsyncDelegate, TDelegate, ComponentSchedulerBase<T, TDelegate>.TaskController, ComponentSchedulerBase<T, TDelegate>.Controller>
        where T : class
    {
        private readonly GlyphAsyncScheduler<T, TAsyncDelegate, TDelegate, TParam> _glyphScheduler;

        public AsyncComponentScheduler(GlyphAsyncScheduler<T, TAsyncDelegate, TDelegate, TParam> glyphScheduler)
            : base(glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;
        }

        public void RunSchedule(TParam param) => _glyphScheduler.RunSchedule(param);
        public void RunSchedule(TParam param, CancellationToken cancellationToken) => _glyphScheduler.RunSchedule(param, cancellationToken);
        public Task RunScheduleAsync(TParam param) => _glyphScheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _glyphScheduler.RunScheduleAsync(param, cancellationToken);

        public TaskController Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate)
        {
            _glyphScheduler.Plan(asyncTaskDelegate, taskDelegate);

            T task = _glyphScheduler.GetDelegateTask(taskDelegate);
            InitPriority(task);

            return new TaskController(this, _glyphScheduler, new[] { task });
        }

        void IGlyphAsyncDelegateScheduler<T, TAsyncDelegate, TDelegate>.Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate)
            => Plan(asyncTaskDelegate, taskDelegate);
    }
}