using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition.Exceptions;

namespace Glyph.Composition.Scheduler.Base
{
    public class DependencyGraph<T> : IDependencyGraph<T>
    {
        private readonly IDictionary<T, DependencyGraphNode<T>> _nodes;
        private readonly IDictionary<Priority, IList<T>> _priorities;
        private readonly List<T> _topologicalOrder;
        private readonly IEnumerable<T> _readOnlyTopologicalOrder;
        private bool _batchMode;
        public int Count { get; private set; }

        public IEnumerable<T> TopologicalOrder
        {
            get
            {
                if (_batchMode)
                    throw new InvalidOperationException(
                        "Dependency graph is currently in edition mode ! Call Close() to exit.");

                return _readOnlyTopologicalOrder;
            }
        }

        public DependencyGraph()
        {
            _nodes = new Dictionary<T, DependencyGraphNode<T>>();

            _priorities = new Dictionary<Priority, IList<T>>
            {
                {Priority.High, new List<T>()},
                {Priority.Normal, new List<T>()},
                {Priority.Low, new List<T>()}
            };

            _topologicalOrder = new List<T>();
            _readOnlyTopologicalOrder = _topologicalOrder.AsReadOnly();
        }

        public IDependencyGraphNode<T> this[T item]
        {
            get { return _nodes[item]; }
        }

        public void BatchStart()
        {
            _batchMode = true;
        }

        public void BatchEnd()
        {
            _batchMode = false;
            Refresh();
        }

        public void AddItem(T item, Priority priority = Priority.Normal, params T[] dependencies)
        {
            if (_nodes.ContainsKey(item))
                throw new ArgumentException("Item provided is already in the dependency graph !");

            var node = new DependencyGraphNode<T>(item, priority, dependencies);
            _nodes.Add(item, node);
            _priorities[priority].Add(item);
            _topologicalOrder.Add(item);

            foreach (T dependency in dependencies)
                AddDependencyInternal(item, dependency);

            Count += dependencies.Length;

            Refresh();
        }

        public void RemoveItem(T item)
        {
            if (!_nodes.ContainsKey(item))
                throw new KeyNotFoundException("Item provided is not in the dependency graph !");

            Count -= _nodes[item].Dependencies.Count;

            _priorities[_nodes[item].Priority].Add(item);
            _nodes.Remove(item);

            Refresh();
        }

        public bool ContainsItem(T item)
        {
            return _nodes.ContainsKey(item);
        }

        public void ClearItems()
        {
            _topologicalOrder.Clear();

            foreach (IList<T> list in _priorities.Values)
                list.Clear();

            _nodes.Clear();
        }

        public void AddDependency(T dependent, T dependency)
        {
            AddDependencyInternal(dependent, dependency);
            Refresh();
        }

        public void RemoveDependency(T dependent, T dependency)
        {
            if (!_nodes.ContainsKey(dependent))
                throw new KeyNotFoundException("Dependent item provided is not in the dependency graph !");

            if (!_nodes.ContainsKey(dependency))
                throw new KeyNotFoundException("Dependency item provided is not in the dependency graph !");

            if (!_nodes[dependent].ContainsDependency(dependency))
                throw new ArgumentException("This dependency relation doesn't exists !");

            _nodes[dependent].RemoveDependency(dependency);
            Count--;
        }

        public bool ContainsDependency(T dependent, T dependency)
        {
            return ContainsItem(dependent) && _nodes[dependent].ContainsDependency(dependency);
        }

        public void ClearDependencies(T item)
        {
            if (!_nodes.ContainsKey(item))
                throw new KeyNotFoundException("Dependent item provided is not in the dependency graph !");

            _nodes[item].ClearDependencies();
        }

        public void ClearDependencies()
        {
            foreach (DependencyGraphNode<T> item in _nodes.Values)
                item.ClearDependencies();

            Refresh();
        }

        public void SetPriority(T item, Priority priority)
        {
            if (!_nodes.ContainsKey(item))
                throw new KeyNotFoundException("Dependency item provided is not in the dependency graph !");

            if (_priorities[priority].Contains(item))
                return;

            _priorities[_nodes[item].Priority].Remove(item);
            _priorities[priority].Add(item);

            Refresh();
        }

        public IEnumerator<IDependencyGraphNode<T>> GetEnumerator()
        {
            return _nodes.Values.GetEnumerator();
        }

        private void AddDependencyInternal(T dependent, T dependency)
        {
            if (!_nodes.ContainsKey(dependent))
                throw new KeyNotFoundException("Dependent item provided is not in the dependency graph !");

            if (!_nodes.ContainsKey(dependency))
                throw new KeyNotFoundException("Dependency item provided is not in the dependency graph !");

            if (_nodes[dependent].ContainsDependency(dependency))
                return;

            _nodes[dependent].AddDependency(dependency);
            Count++;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Refresh()
        {
            if (_batchMode)
                return;

            _topologicalOrder.Clear();

            var visited = new Stack<T>();

            foreach (IList<T> prioritizedItems in _priorities.Values)
                foreach (T item in prioritizedItems)
                {
                    Visit(item, _topologicalOrder, visited);

                    if (_topologicalOrder.Count >= _nodes.Count)
                        break;
                }
        }

        private void Visit(T item, ICollection<T> topologicalOrder, Stack<T> visited)
        {
            if (visited.Contains(item))
                throw new CyclicDependencyException(visited.Cast<object>());

            if (topologicalOrder.Contains(item))
                return;

            visited.Push(item);

            foreach (T dependency in _nodes[item].Dependencies)
                Visit(dependency, topologicalOrder, visited);

            visited.Pop();
            topologicalOrder.Add(item);
        }
    }
}