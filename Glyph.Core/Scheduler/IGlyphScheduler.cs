using System;
using System.Collections.Generic;

namespace Glyph.Core.Scheduler
{
    public interface IGlyphScheduler
    {
    }

    public interface IGlyphScheduler<in T> : IGlyphScheduler
    {
        void Plan(T task);
        void Plan(IEnumerable<T> tasks);
        void Unplan(T task);
    }

    public interface IGlyphScheduler<in T, in TDelegate> : IGlyphScheduler<T>
    {
        void Plan(TDelegate taskDelegate);
        void Unplan(TDelegate taskDelegate);
    }

    public interface IGlyphScheduler<in T, out TTaskController, out TTypeController> : IGlyphScheduler<T>
    {
        new TTaskController Plan(T task);
        new TTaskController Plan(IEnumerable<T> task);
        TTypeController Plan<TTasks>();
        TTypeController Plan(Type taskType);
    }

    public interface IGlyphScheduler<in T, in TDelegate, out TTaskController, out TTypeController> : IGlyphScheduler<T, TDelegate>, IGlyphScheduler<T, TTaskController, TTypeController>
    {
        new TTaskController Plan(TDelegate taskDelegate);
    }
}