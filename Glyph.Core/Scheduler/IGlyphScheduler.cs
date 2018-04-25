using Glyph.Composition;
using Taskete;

namespace Glyph.Core.Scheduler
{
    public interface IGlyphScheduler<out TController, in TInterface, TDelegate> : IScheduler<TController, TDelegate>, IGlyphSchedulerAssigner
        where TController : IGlyphSchedulerController<TController, TInterface, TDelegate>
        where TInterface : IGlyphComponent
    {
        TController Plan(TInterface item);
        void Unplan(TInterface item);
    }
}