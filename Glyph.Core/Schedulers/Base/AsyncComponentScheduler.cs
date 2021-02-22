using System.Threading;
using System.Threading.Tasks;
using Glyph.Scheduling.Base;

namespace Glyph.Core.Schedulers.Base
{
    public class AsyncComponentScheduler<T, TDelegate, TParam> : ComponentSchedulerBase<T, TDelegate>
    {
        private readonly AsyncGlyphScheduler<T, TDelegate, TParam> _glyphScheduler;

        public AsyncComponentScheduler(AsyncGlyphScheduler<T, TDelegate, TParam> glyphScheduler)
            : base(glyphScheduler)
        {
            _glyphScheduler = glyphScheduler;
        }

        public Task RunScheduleAsync(TParam param) => _glyphScheduler.RunScheduleAsync(param);
        public Task RunScheduleAsync(TParam param, CancellationToken cancellationToken) => _glyphScheduler.RunScheduleAsync(param, cancellationToken);
    }
}