using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Diese;
using Glyph.Core.Tracking;
using Glyph.Math;
using Glyph.Messaging;
using Microsoft.Xna.Framework;
using OverGraphed;

namespace Glyph.Core
{
    public class ProjectionManager
    {
        private readonly RootView _rootView;
        private readonly MessagingTracker<IView> _views;

        public IEnumerable<IView> Views
        {
            get
            {
                yield return _rootView;
                foreach (IView view in _views)
                    yield return view;
            }
        }

        public ProjectionManager(RootView rootView, ISubscribableRouter subscribableRouter)
        {
            _rootView = rootView;
            _views = new MessagingTracker<IView>(subscribableRouter);
        }

        public IProjectionController<Transformation> ProjectFrom(IView view, Transformation transformationOnView)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Transformation>(viewVertices[view], transformationOnView, new TransformationProjectionVisitor(), viewVertices, sceneVertices);
        }

        public IProjectionController<Transformation> ProjectFrom(ISceneNode sceneNode, Transformation transformation)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Transformation>(sceneVertices[sceneNode.RootNode()], transformation, new TransformationProjectionVisitor(), viewVertices, sceneVertices);
        }

        public IProjectionController<Transformation> ProjectFrom(ISceneNode sceneNode)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Transformation>(sceneVertices[sceneNode.RootNode()], sceneNode.Transformation, new TransformationProjectionVisitor(), viewVertices, sceneVertices);
        }

        public IProjectionController<Vector2> ProjectFromPosition(IView view, Vector2 positionOnView)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Vector2>(viewVertices[view], positionOnView, new PositionProjectionVisitor(), viewVertices, sceneVertices);
        }

        public IProjectionController<Vector2> ProjectFromPosition(ISceneNode sceneNode, Vector2 position)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Vector2>(sceneVertices[sceneNode.RootNode()], position, new PositionProjectionVisitor(), viewVertices, sceneVertices);
        }

        public IProjectionController<Vector2> ProjectFromPosition(ISceneNode sceneNode)
        {
            BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices);
            return new ProjectionBuilder<Vector2>(sceneVertices[sceneNode.RootNode()], sceneNode.Position, new PositionProjectionVisitor(), viewVertices, sceneVertices);
        }

        private void BuildProjectionGraph(out Dictionary<IView, ViewVertex> viewVertices, out Dictionary<ISceneNode, SceneVertex> sceneVertices)
        {
            viewVertices = new Dictionary<IView, ViewVertex>();
            sceneVertices = new Dictionary<ISceneNode, SceneVertex>(new RepresentativeEqualityComparer<ISceneNode>());

            foreach (IView view in Views)
            {
                if (view.Camera == null)
                    continue;

                var viewVertex = new ViewVertex(view);
                viewVertices[view] = viewVertex;

                ISceneNode viewSceneRoot = view.GetSceneNode().RootNode();
                if (viewSceneRoot != null)
                {
                    if (!sceneVertices.TryGetValue(viewSceneRoot, out SceneVertex viewSceneVertex))
                    {
                        sceneVertices[viewSceneRoot] = viewSceneVertex = new SceneVertex(viewSceneRoot);
                    }

                    new SceneToViewEdge().Link(viewSceneVertex, viewVertex);
                }

                ISceneNode cameraSceneRoot = view.Camera.GetSceneNode().RootNode();
                if (!sceneVertices.TryGetValue(cameraSceneRoot, out SceneVertex cameraSceneVertex))
                {
                    sceneVertices[cameraSceneRoot] = cameraSceneVertex = new SceneVertex(cameraSceneRoot);
                }

                new ViewToSceneEdge().Link(viewVertex, cameraSceneVertex);
            }
        }

        private class RepresentativeEqualityComparer<T> : IEqualityComparer<IRepresentative<T>>
        {
            public bool Equals(IRepresentative<T> x, IRepresentative<T> y)
            {
                return (x?.Represent(y) ?? false) || (y?.Represent(x) ?? false);
            }

            public int GetHashCode(IRepresentative<T> obj)
            {
                return obj.GetHashCode();
            }
        }

        public interface ITargetController<TValue> : IEnumerable<Projection<TValue>>
        {
            IOptionsController<TValue> To(IView targetView);
            IOptionsController<TValue> To(ISceneNode targetSceneNode);
        }

        public interface IOptionsController<TValue> : IEnumerable<Projection<TValue>>
        {
            IOptionsController<TValue> InDirections(GraphDirections directions);
            IOptionsController<TValue> WithDepthMax(int depthMax);
            IOptionsController<TValue> WithViewDepthMax(int viewDepthMax);
        }

        public interface IProjectionController<TValue> : ITargetController<TValue>, IOptionsController<TValue>
        {
        }

        private class ProjectionBuilder<TValue> : IProjectionController<TValue>
        {
            private readonly ProjectionVisitor<TValue> _visitor;
            private readonly Dictionary<IView, ViewVertex> _viewVertices;
            private readonly Dictionary<ISceneNode, SceneVertex> _sceneVertices;
            
            public IVertexBase Source { get; }
            public TValue Value { get; }
            public IVertexBase Target { get; private set; }
            public GraphDirections Directions { get; set; } = GraphDirections.All;
            public int DepthMax { get; set; } = -1;
            public int ViewDepthMax { get; set; } = 1;

            public ProjectionBuilder(IVertexBase source, TValue value, ProjectionVisitor<TValue> visitor, Dictionary<IView, ViewVertex> viewVertices, Dictionary<ISceneNode, SceneVertex> sceneVertices)
            {
                Source = source;
                Value = value;

                _visitor = visitor;
                _viewVertices = viewVertices;
                _sceneVertices = sceneVertices;
            }

            IOptionsController<TValue> ITargetController<TValue>.To(IView targetView)
            {
                Target = _viewVertices[targetView];
                return this;
            }

            IOptionsController<TValue> ITargetController<TValue>.To(ISceneNode targetSceneNode)
            {
                Target = _sceneVertices[targetSceneNode.RootNode()];
                return this;
            }

            IOptionsController<TValue> IOptionsController<TValue>.InDirections(GraphDirections directions)
            {
                Directions = directions;
                return this;
            }

            IOptionsController<TValue> IOptionsController<TValue>.WithDepthMax(int depthMax)
            {
                DepthMax = depthMax;
                return this;
            }

            IOptionsController<TValue> IOptionsController<TValue>.WithViewDepthMax(int viewDepthMax)
            {
                ViewDepthMax = viewDepthMax;
                return this;
            }

            public IEnumerator<Projection<TValue>> GetEnumerator()
            {
                var arguments = new ProjectionVisitor<TValue>.Arguments(Value, Target, Directions, DepthMax, ViewDepthMax);
                if (Source is ViewVertex viewVertex)
                    arguments.ViewDepths[viewVertex] = 1;

                return _visitor.Visit(Source, arguments).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public interface IVertexBase : ILinkableVertex<IVertexBase, IEdgeBase>
        {
            ITransformer Transformer { get; }
            IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args);
        }

        public abstract class VertexBase : SimpleDirectedVertex<IVertexBase, IEdgeBase>, IVertexBase
        {
            protected abstract ITransformer Transformer { get; }
            ITransformer IVertexBase.Transformer => Transformer;

            public abstract IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args);
        }

        public class SceneVertex : VertexBase
        {
            public ISceneNode SceneRoot { get; }
            protected override ITransformer Transformer => SceneRoot;

            public SceneVertex(ISceneNode sceneRoot) => SceneRoot = sceneRoot;
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args) => visitor.Visit(this, args);
        }

        public class ViewVertex : VertexBase
        {
            public IView View { get; }
            protected override ITransformer Transformer => View;

            public ViewVertex(IView view) => View = view;
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args) => visitor.Visit(this, args);
        }

        public interface IEdgeBase : ILinkableEdge<IVertexBase, IEdgeBase>, ITransformer
        {
            IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, bool inverseDirection);
        }

        public abstract class EdgeBase<TStart, TEnd> : Edge<TStart, TEnd, IVertexBase, IEdgeBase>, IEdgeBase
            where TStart : class, IVertexBase where TEnd : class, IVertexBase
        {
            public abstract IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, bool inverseDirection);
            public abstract Vector2 Transform(Vector2 position);
            public abstract Vector2 InverseTransform(Vector2 position);
            public abstract Transformation Transform(Transformation transformation);
            public abstract Transformation InverseTransform(Transformation transformation);
        }

        public class ViewToSceneEdge : EdgeBase<ViewVertex, SceneVertex>
        {
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, bool inverseDirection) => visitor.Visit(this, args, inverseDirection);

            public override Vector2 Transform(Vector2 position)
            {
                IView view = Start.View;
                position = view.Transform(position);
                position = view.Camera.Transform(position);
                return position;
            }

            public override Transformation Transform(Transformation transformation)
            {
                IView view = Start.View;
                transformation = view.Transform(transformation);
                transformation = view.Camera.Transform(transformation);
                return transformation;
            }

            public override Vector2 InverseTransform(Vector2 position)
            {
                IView view = Start.View;
                position = view.Camera.InverseTransform(position);
                position = view.InverseTransform(position);
                return position;
            }

            public override Transformation InverseTransform(Transformation transformation)
            {
                IView view = Start.View;
                transformation = view.Camera.InverseTransform(transformation);
                transformation = view.InverseTransform(transformation);
                return transformation;
            }
        }

        public class SceneToViewEdge : EdgeBase<SceneVertex, ViewVertex>
        {
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, bool inverseDirection) => visitor.Visit(this, args, inverseDirection);

            public override Vector2 Transform(Vector2 position)
            {
                IView view = End.View;

                SceneNode viewSceneNode = view.GetSceneNode();
                if (viewSceneNode != null)
                    position = viewSceneNode.InverseTransform(position);

                position = view.InverseTransform(position);

                return position;
            }

            public override Transformation Transform(Transformation transformation)
            {
                IView view = End.View;

                SceneNode viewSceneNode = view.GetSceneNode();
                if (viewSceneNode != null)
                    transformation = viewSceneNode.InverseTransform(transformation);

                transformation = view.InverseTransform(transformation);

                return transformation;
            }

            public override Vector2 InverseTransform(Vector2 position)
            {
                IView view = End.View;

                position = view.Transform(position);

                SceneNode viewSceneNode = view.GetSceneNode();
                if (viewSceneNode != null)
                    position = viewSceneNode.Transform(position);

                return position;
            }

            public override Transformation InverseTransform(Transformation transformation)
            {
                IView view = End.View;

                transformation = view.Transform(transformation);

                SceneNode viewSceneNode = view.GetSceneNode();
                if (viewSceneNode != null)
                    transformation = viewSceneNode.Transform(transformation);

                return transformation;
            }
        }

        public class PositionProjectionVisitor : ProjectionVisitor<Vector2>
        {
            public PositionProjectionVisitor()
                : base((t, x) => t.Transform(x), (t, x) => t.InverseTransform(x))
            {
            }
        }

        public class TransformationProjectionVisitor : ProjectionVisitor<Transformation>
        {
            public TransformationProjectionVisitor()
                : base((t, x) => t.Transform(x), (t, x) => t.InverseTransform(x))
            {
            }
        }

        public class ProjectionVisitor<TValue>
        {
            private readonly Func<ITransformer, TValue, TValue> _transform;
            private readonly Func<ITransformer, TValue, TValue> _inverseTransform;

            protected ProjectionVisitor(Func<ITransformer, TValue, TValue> transform, Func<ITransformer, TValue, TValue> inverseTransform)
            {
                _transform = transform;
                _inverseTransform = inverseTransform;
            }

            public IEnumerable<Projection<TValue>> Visit(IVertexBase vertex, Arguments args)
            {
                args.TransformerPath.Push(vertex.Transformer);

                if (vertex == args.Target)
                {
                    yield return new Projection<TValue> { Value = args.Value, TransformerPath = args.TransformerPath.ToArray() };
                    yield break;
                }

                if ((args.Directions & GraphDirections.Successors) != 0)
                    foreach (Projection<TValue> projection in VisitInternal(vertex, args, false))
                        yield return projection;

                if ((args.Directions & GraphDirections.Predecessors) != 0)
                    foreach (Projection<TValue> projection in VisitInternal(vertex, args, true))
                        yield return projection;

                args.TransformerPath.Pop();
            }

            public IEnumerable<Projection<TValue>> VisitInternal(IVertexBase vertex, Arguments args, bool inverseDirection)
            {
                IReadOnlyCollection<IEdgeBase> nextVertices = inverseDirection ? vertex.Predecessors : vertex.Successors;

                if (args.Target == null && nextVertices.Count == 0)
                {
                    yield return new Projection<TValue> { Value = args.Value, TransformerPath = args.TransformerPath.ToArray() };
                }
                else
                {
                    foreach (Projection<TValue> value in nextVertices.Where(x => (inverseDirection ? x.Start : x.End) != vertex)
                                                                     .SelectMany(x => x.Accept(this, args, inverseDirection))
                                                                     .Where(x => x.TransformerPath[0] == args.Target.Transformer))
                        yield return value;
                }
            }
            
            public IEnumerable<Projection<TValue>> Visit(ViewVertex vertex, Arguments args)
            {
                if (CheckDepth(args, vertex))
                    yield break;

                if (!args.ViewDepths.ContainsKey(vertex))
                    args.ViewDepths[vertex] = 0;
                args.ViewDepths[vertex]++;

                IVertexBase vertexBase = vertex;
                foreach (Projection<TValue> projection in Visit(vertexBase, args))
                    yield return projection;

                args.ViewDepths[vertex]--;
            }

            public IEnumerable<Projection<TValue>> Visit(IEdgeBase edge, Arguments args, bool inverseDirection)
            {
                Func<ITransformer, TValue, TValue> transformFunc = inverseDirection ? _inverseTransform : _transform;
                IVertexBase nextVertex = inverseDirection ? edge.Start : edge.End;
                args = new Arguments(args);
                
                args.Value = transformFunc(edge, args.Value);

                foreach (Projection<TValue> projection in nextVertex.Accept(this, args))
                    yield return projection;
            }

            private bool CheckDepth(Arguments args, ViewVertex view)
            {
                return args.DepthMax > -1 && args.Depth + 1 >= args.DepthMax
                       || args.ViewDepthMax > -1 && args.ViewDepths.TryGetValue(view, out int viewCount) && viewCount + 1 > args.ViewDepthMax;
            }

            public class Arguments
            {
                public TValue Value { get; set; }
                public IVertexBase Target { get; }
                public GraphDirections Directions { get; }
                public int DepthMax { get; }
                public int ViewDepthMax { get; }

                public Stack<ITransformer> TransformerPath { get; }
                public Dictionary<ViewVertex, int> ViewDepths { get; }
                public int Depth => TransformerPath.Count;

                public Arguments(TValue initialValue, IVertexBase target, GraphDirections directions, int depthMax, int viewDepthMax)
                {
                    Value = initialValue;
                    Target = target;
                    Directions = directions;
                    DepthMax = depthMax;
                    ViewDepthMax = viewDepthMax;

                    TransformerPath = new Stack<ITransformer>();
                    ViewDepths = new Dictionary<ViewVertex, int>();
                }

                public Arguments(Arguments arguments)
                {
                    Target = arguments.Target;
                    Value = arguments.Value;
                    Directions = arguments.Directions;
                    DepthMax = arguments.DepthMax;
                    ViewDepthMax = arguments.ViewDepthMax;

                    TransformerPath = arguments.TransformerPath;
                    ViewDepths = arguments.ViewDepths;
                }
            }
        }
    }

    [Flags]
    public enum GraphDirections
    {
        Successors = 1 << 0,
        Predecessors = 1 << 1,
        All = Successors | Predecessors
    }
}