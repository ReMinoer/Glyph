using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Scheduler.Base
{
    public class DependencyGraphNode<T> : IDependencyGraphNode<T>
    {
        private readonly List<T> _dependencies;
        private readonly IReadOnlyCollection<T> _dependenciesReadOnly;
        public T Item { get; private set; }
        public Priority Priority { get; private set; }

        public IReadOnlyCollection<T> Dependencies
        {
            get { return _dependenciesReadOnly; }
        }

        public DependencyGraphNode(T item, Priority priority = Priority.Normal, params T[] dependencies)
        {
            Item = item;
            Priority = priority;
            _dependencies = dependencies.ToList();
            _dependenciesReadOnly = _dependencies.AsReadOnly();
        }

        internal void AddDependency(T dependency)
        {
            _dependencies.Add(dependency);
        }

        internal void RemoveDependency(T dependency)
        {
            _dependencies.Remove(dependency);
        }

        internal bool ContainsDependency(T dependency)
        {
            return _dependencies.Contains(dependency);
        }

        internal void ClearDependencies()
        {
            _dependencies.Clear();
        }
    }
}