using System;
using System.Collections.Generic;

namespace Glyph.Scheduling
{
    public interface IGlyphScheduler
    {
    }

    public interface IGlyphScheduler<in T> : IGlyphScheduler
    {
        void Plan(T task);
        void Unplan(T task);
    }

    public interface IGlyphScheduler<in T, out TController> : IGlyphScheduler<T>
    {
        new TController Plan(T task);
        TController Plan(IEnumerable<T> tasks);
    }

    public interface IGlyphScheduler<in T, out TTaskController, out TTypeController> : IGlyphScheduler<T, TTaskController>
    {
        TTypeController Plan(Type taskType);
        TTypeController Plan<TTask>();
    }

    public interface IGlyphDelegateScheduler<in T, in TDelegate> : IGlyphScheduler<T>
    {
        void Plan(TDelegate taskDelegate);
        void Unplan(TDelegate taskDelegate);
    }

    public interface IGlyphDelegateScheduler<in T, in TDelegate, out TTaskController, out TTypeController>
        : IGlyphDelegateScheduler<T, TDelegate>, IGlyphScheduler<T, TTaskController, TTypeController>
    {
        new TTaskController Plan(TDelegate taskDelegate);
    }

    public interface IGlyphAsyncDelegateScheduler<in T, in TAsyncDelegate, in TDelegate> : IGlyphDelegateScheduler<T, TDelegate>
    {
        void Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate);
    }

    public interface IGlyphAsyncDelegateScheduler<in T, in TAsyncDelegate, in TDelegate, out TTaskController, out TTypeController>
        : IGlyphAsyncDelegateScheduler<T, TAsyncDelegate, TDelegate>, IGlyphScheduler<T, TTaskController, TTypeController>
    {
        new TTaskController Plan(TAsyncDelegate asyncTaskDelegate, TDelegate taskDelegate);
    }
}