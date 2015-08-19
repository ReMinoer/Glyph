using Glyph.Composition.Scheduler.Base;

namespace Glyph.Composition.Scheduler
{
    public interface IGlyphScheduler<in TInterface, TDelegate> : IScheduler<TDelegate>, IGlyphSchedulerAssigner
        where TInterface : IGlyphComponent
    {
        new IGlyphSchedulerController<TInterface, TDelegate> Plan(TDelegate item);
        IGlyphSchedulerController<TInterface, TDelegate> Plan(TInterface item);
        void Unplan(TInterface item);
    }
}