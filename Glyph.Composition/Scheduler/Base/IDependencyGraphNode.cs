using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public interface IDependencyGraphNode<out T>
    {
        T Item { get; }
        IReadOnlyCollection<T> Dependencies { get; }
        Priority Priority { get; }
    }
}