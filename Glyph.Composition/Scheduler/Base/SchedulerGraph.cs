using System;
using System.Collections.Generic;
using Diese.Graph;

namespace Glyph.Composition.Scheduler.Base
{
    public class SchedulerGraph<T> :
        GraphBase<SchedulerGraph<T>, SchedulerGraph<T>.Vertex, SchedulerGraph<T>.Edge, SchedulerGraph<T>.Visitor>
    {
        public class Vertex : VertexBase<SchedulerGraph<T>, Vertex, Edge, Visitor>
        {
            public Func<object, bool> Predicate { get; set; } 
            public IList<T> Items { get; private set; }
            public Priority Priority { get; set; }

            public Vertex(Func<object, bool> predicate)
            {
                Predicate = predicate;

                Items = new List<T>();
                Priority = Priority.Normal;
            }

            public Vertex(T item)
            {
                Items = new List<T> { item };

                Predicate = x => false;
                Priority = Priority.Normal;
            }
        }

        public class Edge : EdgeBase<SchedulerGraph<T>, Vertex, Edge, Visitor>
        {
            public Edge(Vertex start, Vertex end)
                : base(start, end)
            {
            }
        }

        public abstract class Visitor : VisitorBase<SchedulerGraph<T>, Vertex, Edge, Visitor>
        {
        }
    }
}