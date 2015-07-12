using Diese.Graph;

namespace Glyph.Composition.Scheduler.Base
{
    public class DependencyGraph<T> :
        GraphBase<DependencyGraph<T>, DependencyGraph<T>.Vertex, DependencyGraph<T>.Edge, DependencyGraph<T>.Visitor>
    {
        public class Vertex : VertexBase<DependencyGraph<T>, Vertex, Edge, Visitor>
        {
            public T Item { get; private set; }
            public Priority Priority { get; set; }

            public Vertex(T item, Priority priority = Priority.Normal)
            {
                Item = item;
                Priority = priority;
            }
        }

        public class Edge : EdgeBase<DependencyGraph<T>, Vertex, Edge, Visitor>
        {
            public Edge(Vertex start, Vertex end)
                : base(start, end)
            {
            }
        }

        public abstract class Visitor : VisitorBase<DependencyGraph<T>, Vertex, Edge, Visitor>
        {
        }
    }
}