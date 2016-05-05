using Diese.Injection;
using Glyph.Composition.Delegates;
using Glyph.Composition.Scheduler;

namespace Glyph.Composition
{
    // TASK : get Injectable at runtime
    public class GlyphObject : GlyphSchedulableBase
    {
        protected readonly SchedulerHandler Schedulers;

        protected override sealed SchedulerHandlerBase SchedulerAssigner
        {
            get { return Schedulers; }
        }

        public GlyphObject(IDependencyInjector injector)
            : base(injector)
        {
            Schedulers = new SchedulerHandler(injector);
        }

        public override sealed void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            foreach (DrawDelegate draw in Schedulers.Draw.TopologicalOrder)
                draw(drawer);
        }

        protected class SchedulerHandler : SchedulerHandlerBase
        {
            public IGlyphScheduler<IDraw, DrawDelegate> Draw { get; private set; }

            public SchedulerHandler(IDependencyInjector injector)
                : base(injector)
            {
                Draw = Add<IDraw, DrawDelegate>();
            }
        }
    }
}