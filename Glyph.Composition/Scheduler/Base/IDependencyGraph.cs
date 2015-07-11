using System.Collections.Generic;

namespace Glyph.Composition.Scheduler.Base
{
    public interface IDependencyGraph<T> : IEnumerable<IDependencyGraphNode<T>>
    {
        int Count { get; }
        IEnumerable<T> TopologicalOrder { get; }
        IDependencyGraphNode<T> this[T item] { get; }
        void BatchStart();
        void BatchEnd();
        void AddItem(T item, Priority priority = Priority.Normal, params T[] dependencies);
        void RemoveItem(T item);
        bool ContainsItem(T item);
        void ClearItems();
        void AddDependency(T dependent, T dependency);
        void RemoveDependency(T dependent, T dependency);
        bool ContainsDependency(T dependent, T dependency);
        void ClearDependencies(T item);
        void ClearDependencies();
        void SetPriority(T item, Priority priority);
    }
}