using System.Collections.Generic;
using System.Linq;

namespace Glyph.Composition.Scheduler.Base
{
    public class TopologicalOrderVisitor<T> : SchedulerGraph<T>.Visitor
    {
        private readonly List<T> _result;
        private readonly IReadOnlyCollection<T> _readOnlyResult;
        private readonly Stack<SchedulerGraph<T>.Vertex> _visited;

        public IEnumerable<T> Result
        {
            get { return _readOnlyResult; }
        }

        public TopologicalOrderVisitor()
        {
            _result = new List<T>();
            _readOnlyResult = _result.AsReadOnly();

            _visited = new Stack<SchedulerGraph<T>.Vertex>();
        }

        public override void Process(SchedulerGraph<T> graph)
        {
            _result.Clear();
            _visited.Clear();

            foreach (IGrouping<Priority, SchedulerGraph<T>.Vertex> group in graph.Vertices.GroupBy(x => x.Priority))
                foreach (SchedulerGraph<T>.Vertex vertex in group)
                {
                    vertex.Accept(this);

                    if (_result.Count >= graph.Vertices.Count)
                        break;
                }
        }

        public override void Visit(SchedulerGraph<T>.Vertex vertex)
        {
            if (_visited.Contains(vertex))
                throw new CyclicDependencyException(_visited.Select(x => x.Items.First()).Cast<object>());

            if (_result.Contains(vertex.Items.First()))
                return;

            _visited.Push(vertex);

            foreach (SchedulerGraph<T>.Vertex successor in vertex.Successors)
                successor.Accept(this);

            _visited.Pop();

            _result.AddRange(vertex.Items);
        }
    }
}