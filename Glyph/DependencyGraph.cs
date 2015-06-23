using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Exceptions;

namespace Glyph
{
    public interface IDependencyGraph<T>
    {
        int Count { get; }

        event EventHandler GraphEdited;

        List<T> this[T item] { get; }
        void AddItem(T item, params T[] dependencies);
        void RemoveItem(T item);
        bool ContainsItem(T item);
        void AddDependency(T dependent, T dependency);
        void RemoveDependency(T dependent, T dependency);
        bool ContainsDependency(T dependent, T dependency);
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

        public int Count { get; private set; }

        public List<T> this[T item]
        {
            get { return _dependencies[item]; }
        }

        public void AddItem(T item, params T[] dependencies)
        {
            if (_dependencies.ContainsKey(item))
                throw new ArgumentException("Item provided is already in the dependency graph !");

            _dependencies.Add(item, new List<T>());

            foreach (T dependency in dependencies)
                AddDependency(item, dependency);

            Count += dependencies.Length;

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(T item)
        {
            if (!_dependencies.ContainsKey(item))
                throw new KeyNotFoundException("Item provided is not in the dependency graph !");

            Count -= _dependencies[item].Count;
            _dependencies.Remove(item);

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public bool ContainsItem(T item)
        {
            return _dependencies.ContainsKey(item);
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
            Count++;

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
            Count--;

            if (GraphEdited != null)
                GraphEdited.Invoke(this, EventArgs.Empty);
        }

        public bool ContainsDependency(T dependent, T dependency)
        {
            return ContainsItem(dependent) && _dependencies[dependent].Contains(dependency);
        }

        public List<T> GetTopologicalOrder()
        {
            var sorted = new List<T>();
            var visited = new Stack<T>();

            foreach (T item in _dependencies.Keys)
            {
                Visit(item, sorted, visited);

                if (sorted.Count == _dependencies.Count)
                    break;
            }

            return sorted;
        }

        private void Visit(T item, ICollection<T> sorted, Stack<T> visited)
        {
            if (visited.Contains(item))
                throw new CyclicDependencyException(visited.Cast<object>());

            visited.Push(item);

            foreach (T dependency in _dependencies[item])
                Visit(dependency, sorted, visited);

            visited.Pop();
            sorted.Add(item);
        }
    }
}