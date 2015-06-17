using System;
using System.Collections.Generic;

namespace Glyph
{
    public interface IDependencyGraph<T>
    {
        event EventHandler GraphEdited;

        void AddItem(T item, params T[] dependencies);
        void RemoveItem(T item);
        void AddDependency(T dependent, T dependency);
        void RemoveDependency(T dependent, T dependency);
        List<T> GetTopologicalOrder();
    }

    public class DependencyGraph<T> : IDependencyGraph<T>
    {
        private readonly Dictionary<T, List<T>> _dependencies;

        public event EventHandler GraphEdited;

        public DependencyGraph()
        {
            _dependencies = new Dictionary<T, List<T>>();
        }

        public void AddItem(T item, params T[] dependencies)
        {
            if (_dependencies.ContainsKey(item))
                throw new ArgumentException("Item provided is already in the dependency graph !");

            _dependencies.Add(item, new List<T>());

            foreach (T dependency in dependencies)
                AddDependency(item, dependency);

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(T item)
        {
            if (!_dependencies.ContainsKey(item))
                throw new KeyNotFoundException("Item provided is not in the dependency graph !");

            _dependencies.Remove(item);

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public void AddDependency(T dependent, T dependency)
        {
            if (!_dependencies.ContainsKey(dependent))
                throw new KeyNotFoundException("Dependent item provided is not in the dependency graph !");

            if (!_dependencies.ContainsKey(dependency))
                throw new KeyNotFoundException("Dependency item provided is not in the dependency graph !");

            if (_dependencies[dependent].Contains(dependency))
                throw new ArgumentException("This dependency relation already exists !");

            _dependencies[dependent].Add(dependency);

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public void RemoveDependency(T dependent, T dependency)
        {
            if (!_dependencies.ContainsKey(dependent))
                throw new KeyNotFoundException("Dependent item provided is not in the dependency graph !");

            if (!_dependencies.ContainsKey(dependency))
                throw new KeyNotFoundException("Dependency item provided is not in the dependency graph !");

            if (!_dependencies[dependent].Contains(dependency))
                throw new ArgumentException("This dependency relation doesn't exists !");

            _dependencies[dependent].Remove(dependency);

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public List<T> GetTopologicalOrder()
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (T item in _dependencies.Keys)
            {
                Visit(item, sorted, visited);

                if (sorted.Count == _dependencies.Count)
                    break;
            }

            return sorted;
        }

        private void Visit(T item, ICollection<T> sorted, IDictionary<T, bool> visited)
        {
            bool visitedDuringCurrentProcess;
            bool alreadyVisited = visited.TryGetValue(item, out visitedDuringCurrentProcess);

            if (alreadyVisited)
            {
                if (visitedDuringCurrentProcess)
                    throw new ArgumentException("Cyclic dependency found.");
            }
            else
            {
                visited[item] = true;

                foreach (T dependency in _dependencies[item])
                    Visit(dependency, sorted, visited);

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}