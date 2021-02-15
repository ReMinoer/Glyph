using System;

namespace Glyph.Core.Scheduler
{
    public interface IGlyphScheduler
    {
    }

    public interface IGlyphScheduler<in T> : IGlyphScheduler
    {
        void Plan(T task);
        void Unplan(T task);
    }

    public interface IGlyphScheduler<in T, out TItemController, out TTypeController> : IGlyphScheduler<T>
    {
        new TItemController Plan(T task);
        TTypeController Plan<TTasks>();
        TTypeController Plan(Type taskType);
    }
}