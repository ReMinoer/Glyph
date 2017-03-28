using System.Collections.Generic;
using Diese;

namespace Glyph.Core.Scheduler.Base
{
    public interface IScheduler<out TController, T> : IBatchTree
    {
        IEnumerable<T> Planning { get; }
        TController Plan(T item);
        void Unplan(T item);
    }
}