using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Scheduler.Base
{
    public class TopologicalOrderVisitor<T> : DependencyGraph<T>.Visitor
    {
        private readonly List<T> _result;
        private readonly IReadOnlyCollection<T> _readOnlyResult;
        private readonly Stack<T> _visited;

        public IEnumerable<T> Result
        {
            get { return _readOnlyResult; }
        }

        public TopologicalOrderVisitor()
        {
            _result = new List<T>();
            _readOnlyResult = _result.AsReadOnly();

            _visited = new Stack<T>();
        }

        public override void Process(DependencyGraph<T> graph)
        {
            _result.Clear();
            _visited.Clear();

            foreach (IGrouping<Priority, DependencyGraph<T>.Vertex> group in graph.Vertices.GroupBy(x => x.Priority))
                foreach (DependencyGraph<T>.Vertex vertex in group)
                {
                    vertex.Accept(this);

                    if (_result.Count >= graph.Vertices.Count)
                        break;
                }
        }

        public override void Visit(DependencyGraph<T>.Vertex vertex)
        {
            if (_visited.Contains(vertex.Item))
                throw new CyclicDependencyException(_visited.Cast<object>());

            if (_result.Contains(vertex.Item))
                return;

            _visited.Push(vertex.Item);

            foreach (DependencyGraph<T>.Vertex successor in vertex.Successors)
                successor.Accept(this);

            _visited.Pop();

            _result.Add(vertex.Item);
        }
    }
}