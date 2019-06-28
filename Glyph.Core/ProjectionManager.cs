﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Composition;
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

        public IEnumerable<ISceneNode> SceneRoots => ViewsSceneRoots.Union(CamerasSceneRoots).Distinct();
        public IEnumerable<ISceneNode> ViewsSceneRoots => Views.Select(x => x.GetSceneNode().RootNode()).NotNulls().Distinct();
        public IEnumerable<ISceneNode> CamerasSceneRoots => Views.Select(x => x.Camera.GetSceneNode().RootNode()).Distinct();

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

        public interface ITargetController<TValue> : IEnumerable<Projection<TValue>>
        {
            IOptionsController<TValue> To(IView targetView);
            IOptionsController<TValue> To(ISceneNode targetSceneNode);
        }

        public interface IOptionsController<TValue> : IEnumerable<Projection<TValue>>
        {
            IRaycastController<TValue> ByRaycast();
            IOptionsController<TValue> InDirections(GraphDirections directions);
            IOptionsController<TValue> WithDepthMax(int depthMax);
            IOptionsController<TValue> WithViewDepthMax(int viewDepthMax);
        }

        public interface IRaycastController<TValue> : IOptionsController<TValue>
        {
            IOptionsController<TValue> ForDrawClient(IDrawClient drawClient);
        }

        public interface IProjectionController<TValue> : ITargetController<TValue>, IOptionsController<TValue>
        {
        }

        private class ProjectionBuilder<TValue> : IProjectionController<TValue>, IRaycastController<TValue>
        {
            private readonly ProjectionVisitor<TValue> _visitor;
            private readonly Dictionary<IView, ViewVertex> _viewVertices;
            private readonly Dictionary<ISceneNode, SceneVertex> _sceneVertices;
            
            public IVertexBase Source { get; }
            public TValue Value { get; }
            public IVertexBase Target { get; private set; }
            public bool Raycasting { get; private set; }
            public IDrawClient RaycastClient { get; private set; }
            public GraphDirections Directions { get; private set; } = GraphDirections.All;
            public int DepthMax { get; private set; } = -1;
            public int ViewDepthMax { get; private set; } = 1;

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

            IRaycastController<TValue> IOptionsController<TValue>.ByRaycast()
            {
                Raycasting = true;
                return this;
            }

            IOptionsController<TValue> IRaycastController<TValue>.ForDrawClient(IDrawClient drawClient)
            {
                RaycastClient = drawClient;
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
                var arguments = new ProjectionVisitor<TValue>.Arguments(Value, Target, Raycasting, RaycastClient, Directions, DepthMax, ViewDepthMax);
                return Source.Accept(_visitor, arguments).GetEnumerator();
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
            IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, GraphDirection direction);
        }

        public abstract class EdgeBase<TStart, TEnd> : Edge<TStart, TEnd, IVertexBase, IEdgeBase>, IEdgeBase
            where TStart : class, IVertexBase where TEnd : class, IVertexBase
        {
            public abstract IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, GraphDirection direction);
            public abstract Vector2 Transform(Vector2 position);
            public abstract Vector2 InverseTransform(Vector2 position);
            public abstract Transformation Transform(Transformation transformation);
            public abstract Transformation InverseTransform(Transformation transformation);
            public event EventHandler TransformationChanged;
        }

        public class ViewToSceneEdge : EdgeBase<ViewVertex, SceneVertex>
        {
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, GraphDirection direction) => visitor.Visit(this, args, direction);

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
            public override IEnumerable<Projection<TValue>> Accept<TValue>(ProjectionVisitor<TValue> visitor, ProjectionVisitor<TValue>.Arguments args, GraphDirection direction) => visitor.Visit(this, args, direction);

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
                : base((t, x) => t.Transform(x), (t, x) => t.InverseTransform(x), (a, x) => a.ContainsPoint(x))
            {
            }
        }

        public class TransformationProjectionVisitor : ProjectionVisitor<Transformation>
        {
            public TransformationProjectionVisitor()
                : base((t, x) => t.Transform(x), (t, x) => t.InverseTransform(x), (a, x) => a.ContainsPoint(x.Translation))
            {
            }
        }

        public class ProjectionVisitor<TValue>
        {
            private readonly Func<ITransformer, TValue, TValue> _transform;
            private readonly Func<ITransformer, TValue, TValue> _inverseTransform;
            private readonly Func<IShape, TValue, bool> _areaContains;

            protected ProjectionVisitor(Func<ITransformer, TValue, TValue> transform, Func<ITransformer, TValue, TValue> inverseTransform, Func<IShape, TValue, bool> areaContains)
            {
                _transform = transform;
                _inverseTransform = inverseTransform;
                _areaContains = areaContains;
            }
            
            public IEnumerable<Projection<TValue>> Visit(SceneVertex vertex, Arguments args)
            {
                IVertexBase vertexBase = vertex;
                foreach (Projection<TValue> projection in Visit(vertexBase, args))
                    yield return projection;
            }
            
            public IEnumerable<Projection<TValue>> Visit(ViewVertex vertex, Arguments args)
            {
                if (CheckDepth(args, vertex))
                    yield break;

                if (args.Raycasting && (!vertex.View.Displayed(drawClient: args.RaycastClient)))// || !_areaContains(vertex.View, args.Value)))
                    yield break;

                if (!args.ViewDepths.ContainsKey(vertex))
                    args.ViewDepths[vertex] = 0;
                args.ViewDepths[vertex]++;

                IVertexBase vertexBase = vertex;
                foreach (Projection<TValue> projection in Visit(vertexBase, args))
                {
                    yield return projection;
                    if (args.Raycasting)
                        break;
                }

                args.ViewDepths[vertex]--;
            }

            private IEnumerable<Projection<TValue>> Visit(IVertexBase vertex, Arguments args)
            {
                args.TransformerPath.Push(vertex.Transformer);

                if (vertex == args.Target)
                {
                    yield return new Projection<TValue> { Value = args.Value, TransformerPath = args.TransformerPath.ToArray() };
                    yield break;
                }

                if ((args.Directions & GraphDirections.Successors) != 0)
                    foreach (Projection<TValue> projection in VisitDirection(vertex, args, GraphDirection.Successors))
                        yield return projection;

                if ((args.Directions & GraphDirections.Predecessors) != 0)
                    foreach (Projection<TValue> projection in VisitDirection(vertex, args, GraphDirection.Predecessors))
                        yield return projection;

                args.TransformerPath.Pop();
            }

            public IEnumerable<Projection<TValue>> VisitDirection(IVertexBase vertex, Arguments args, GraphDirection direction)
            {
                IReadOnlyCollection<IEdgeBase> nextEdges = GetNextEdges(vertex, direction);

                if (args.Target == null && nextEdges.Count == 0)
                {
                    yield return new Projection<TValue> { Value = args.Value, TransformerPath = args.TransformerPath.ToArray() };
                }
                else
                {
                    foreach (Projection<TValue> value in nextEdges.Where(x => GetNextVertex(x, direction) != vertex)
                                                                  .OrderBy(x => (GetNextVertex(x, direction) as ViewVertex)?.View.GetSceneNode()?.Depth ?? 0)
                                                                  .SelectMany(x => x.Accept(this, args, direction))
                                                                  .Where(x => args.Target == null || x.TransformerPath[0] == args.Target.Transformer))
                        yield return value;
                }
            }

            public IEnumerable<Projection<TValue>> Visit(IEdgeBase edge, Arguments args, GraphDirection direction)
            {
                args = new Arguments(args);
                args.Value = Transform(edge, args.Value, direction);
                
                IVertexBase nextVertex = GetNextVertex(edge, direction);
                foreach (Projection<TValue> projection in nextVertex.Accept(this, args))
                    yield return projection;
            }

            private bool CheckDepth(Arguments args, ViewVertex view)
            {
                return args.DepthMax > -1 && args.Depth + 1 >= args.DepthMax
                       || args.ViewDepthMax > -1 && args.ViewDepths.TryGetValue(view, out int viewCount) && viewCount + 1 >= args.ViewDepthMax;
            }

            private TValue Transform(ITransformer transformer, TValue value, GraphDirection direction)
            {
                switch (direction)
                {
                    case GraphDirection.Successors: return _transform(transformer, value);
                    case GraphDirection.Predecessors: return _inverseTransform(transformer, value);
                    default: throw new NotSupportedException();
                }
            }

            static private IReadOnlyCollection<IEdgeBase> GetNextEdges(IVertexBase vertex, GraphDirection direction)
            {
                switch (direction)
                {
                    case GraphDirection.Successors: return vertex.Successors;
                    case GraphDirection.Predecessors: return vertex.Predecessors;
                    default: throw new NotSupportedException();
                }
            }

            static private IVertexBase GetNextVertex(IEdgeBase edge, GraphDirection direction)
            {
                switch (direction)
                {
                    case GraphDirection.Successors: return edge.End;
                    case GraphDirection.Predecessors: return edge.Start;
                    default: throw new NotSupportedException();
                }
            }

            public class Arguments
            {
                public TValue Value { get; set; }
                public IVertexBase Target { get; }
                public bool Raycasting { get; }
                public IDrawClient RaycastClient { get; }
                public GraphDirections Directions { get; }
                public int DepthMax { get; }
                public int ViewDepthMax { get; }

                public Stack<ITransformer> TransformerPath { get; }
                public Dictionary<ViewVertex, int> ViewDepths { get; }
                public int Depth => TransformerPath.Count;

                public Arguments(TValue initialValue, IVertexBase target, bool raycasting, IDrawClient raycastClient, GraphDirections directions, int depthMax, int viewDepthMax)
                {
                    Value = initialValue;
                    Target = target;
                    Raycasting = raycasting;
                    Directions = directions;
                    DepthMax = depthMax;
                    ViewDepthMax = viewDepthMax;
                    RaycastClient = raycastClient;

                    TransformerPath = new Stack<ITransformer>();
                    ViewDepths = new Dictionary<ViewVertex, int>();
                }

                public Arguments(Arguments arguments)
                {
                    Target = arguments.Target;
                    Value = arguments.Value;
                    Raycasting = arguments.Raycasting;
                    RaycastClient = arguments.RaycastClient;
                    Directions = arguments.Directions;
                    DepthMax = arguments.DepthMax;
                    ViewDepthMax = arguments.ViewDepthMax;

                    TransformerPath = arguments.TransformerPath;
                    ViewDepths = arguments.ViewDepths;
                }
            }
        }
    }
    
    public enum GraphDirection
    {
        Successors,
        Predecessors
    }

    [Flags]
    public enum GraphDirections
    {
        Successors = 1 << 0,
        Predecessors = 1 << 1,
        All = Successors | Predecessors
    }
}