using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Diese;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Resolver;
using Niddle.Attributes;
using NLog;
using OverGraphed;

namespace Glyph.Animation
{
    public interface IAnimationGraph : IGraph, IUpdate, ITimeUnscalable
    {
    }

    public interface IAnimationGraph<T, TState> : IAnimationGraph, IGraph<AnimationGraph<T, TState>.Vertex, AnimationGraph<T, TState>.Transition>
        where T : class
    {
    }

    public class AnimationGraph<T, TState> : GlyphContainer<IAnimationPlayer>, IAnimationGraph<T, TState>
        where T : class
    {
        static private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly AutoGraph<Vertex, Transition> _graph;
        private readonly Dictionary<TState, Vertex> _states;
        private readonly ReadOnlyDictionary<TState, Vertex> _readOnlyStates;
        private bool _useUnscaledTime;
        private bool _started;

        [Resolvable, ResolveTargets(ResolveTargets.Parent | ResolveTargets.Fraternal)]
        public T Animatable { get; set; }
        public TState Current { get; private set; }
        public TState Start { get; set; }

        public IReadOnlyDictionary<TState, Vertex> States => _readOnlyStates;
        public IEnumerable<Transition> Transitions => _graph.Edges;

        IEnumerable<Vertex> IGraph<Vertex, Transition>.Vertices => _graph.Vertices;
        IEnumerable<Transition> IGraph<Vertex, Transition>.Edges => _graph.Edges;
        IEnumerable<IVertex> IGraph.Vertices => _graph.Vertices;
        IEnumerable<IEdge> IGraph.Edges => _graph.Edges;

        event Event<Vertex> IGraph<Vertex, Transition>.VertexAdded
        {
            add => _graph.VertexAdded += value;
            remove => _graph.VertexAdded -= value;
        }

        event Event<Vertex> IGraph<Vertex, Transition>.VertexRemoved
        {
            add => _graph.VertexRemoved += value;
            remove => _graph.VertexRemoved -= value;
        }

        event Event<Transition> IGraph<Vertex, Transition>.EdgeAdded
        {
            add => _graph.EdgeAdded += value;
            remove => _graph.EdgeAdded -= value;
        }

        event Event<Transition> IGraph<Vertex, Transition>.EdgeRemoved
        {
            add => _graph.EdgeRemoved += value;
            remove => _graph.EdgeRemoved -= value;
        }

        event Event IGraph.Cleared
        {
            add => _graph.Cleared += value;
            remove => _graph.Cleared -= value;
        }

        event Event<IVertex> IGraph.VertexAdded
        {
            add => ((IGraph)_graph).VertexAdded += value;
            remove => ((IGraph)_graph).VertexAdded -= value;
        }

        event Event<IVertex> IGraph.VertexRemoved
        {
            add => ((IGraph)_graph).VertexRemoved += value;
            remove => ((IGraph)_graph).VertexRemoved -= value;
        }

        event Event<IEdge> IGraph.EdgeAdded
        {
            add => ((IGraph)_graph).EdgeAdded += value;
            remove => ((IGraph)_graph).EdgeAdded -= value;
        }

        event Event<IEdge> IGraph.EdgeRemoved
        {
            add => ((IGraph)_graph).EdgeRemoved += value;
            remove => ((IGraph)_graph).EdgeRemoved -= value;
        }

        public IAnimation<T> this[TState state]
        {
            get => ResolveState(state).Animation;
            set => ResolveState(state).Animation = value;
        }

        public bool UseUnscaledTime
        {
            get => _useUnscaledTime;
            set
            {
                _useUnscaledTime = value;
                foreach (IAnimationPlayer player in Components)
                    player.UseUnscaledTime = value;
            }
        }

        public AnimationGraph()
        {
            _graph = new AutoGraph<Vertex, Transition>();
            _states = new Dictionary<TState, Vertex>();
            _readOnlyStates = new ReadOnlyDictionary<TState, Vertex>(_states);
        }

        public override void Initialize()
        {
            Current = Start;
            _started = false;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            TState previous = Current;

            Vertex vertex = _states[Current];
            State state = vertex.Update(Animatable);
            Current = state.Key;

            if (!_started || !Current.Equals(previous))
            {
                UpdateAnimationPlayer(state);
                Logger.Trace($"{Animatable.GetType().GetDisplayName()} state: {Current}");
                _started = true;
            }

            while (vertex.Successors.Any(x => Components.All(y => y.IsLooping || y.HasEnded) && (x.Predicate == null || x.Predicate(Animatable)) && !x.OnRequest, out Transition transition))
            {
                Current = transition.End.InnerStates.First().Key;

                vertex = _states[Current];
                state = vertex.Update(Animatable);
                Current = state.Key;

                UpdateAnimationPlayer(state);
                Logger.Trace($"{Animatable.GetType().GetDisplayName()} state: {Current}");
            }

            foreach (IAnimationPlayer player in Components)
                player.Update(elapsedTime);
        }

        public bool TryRequestTransitionTo(TState key)
        {
            Vertex vertex = _states[Current];

            if (!vertex.Successors.Any(x => x.OnRequest && x.End.InnerStates.ContainsKey(key), out Transition transition))
                return false;

            vertex = _states[transition.End.InnerStates.First().Key];
            State state = vertex.Update(Animatable);
            Current = state.Key;

            Logger.Trace($"{Animatable.GetType().GetDisplayName()} state: {Current}");

            UpdateAnimationPlayer(state);
            return true;
        }

        public bool CanTransitTo(TState key)
        {
            Vertex vertex = _states[Current];
            return vertex.Successors.Any(x => x.OnRequest && x.End.InnerStates.ContainsKey(key));
        }

        public IStateController GetState(TState state)
        {
            return new StateController(this, ResolveVertex(state));
        }

        public IBeginMetaStateController AddMergedState()
        {
            return new MergedStateController(this);
        }

        public void RemoveState(TState state)
        {
            Vertex vertex = ResolveVertex(state);

            _graph.UnregisterVertex(vertex);
            vertex.UnlinkEdges();

            _states.Remove(state);
        }

        internal Transition AddTransition(Vertex start, Vertex end, Predicate<T> predicate = null)
        {
            var transition = new Transition(predicate);
            transition.Link(start, end);
            return transition;
        }

        public void RemoveTransition(TState start, TState end)
        {
            Vertex startVertex = ResolveVertex(start);
            Vertex endVertex = ResolveVertex(end);
            startVertex.Successors.First(x => x.End == endVertex).Unlink();
        }

        public void ContainsTransition(TState start, TState end)
        {
            _graph.ContainsLink(ResolveVertex(start), ResolveVertex(end));
        }

        private void UpdateAnimationPlayer(State state)
        {
            Components.Clear();

            var animationPlayer = new AnimationPlayer<T>(Animatable)
            {
                Animation = state.Animation,
                TimeOffset = 0
            };
            animationPlayer.Play();

            Components.Add(animationPlayer);
        }

        private Vertex ResolveVertex(TState state)
        {
            if (!_states.TryGetValue(state, out Vertex vertex))
                _graph.RegisterVertex(_states[state] = vertex = new SingleStateVertex(state));
            return vertex;
        }

        private State ResolveState(TState state)
        {
            if (_states.TryGetValue(state, out Vertex vertex))
                return vertex.InnerStates[state];

            var newState = new SingleStateVertex(state);
            _states[state] = newState;
            _graph.RegisterVertex(newState);
            return newState.Update(Animatable);
        }

        public class State
        {
            public TState Key { get; }
            public IAnimation<T> Animation { get; set; }

            public State(TState key)
            {
                Key = key;
            }
        }

        public abstract class Vertex : SimpleDirectedVertex<Vertex, Transition>
        {
            protected internal abstract IReadOnlyDictionary<TState, State> InnerStates { get; }
            public abstract State Update(T animatable);
        }

        public class SingleStateVertex : Vertex
        {
            private State State { get; }
            private readonly Dictionary<TState, State> _innerStates = new Dictionary<TState, State>();
            protected internal override IReadOnlyDictionary<TState, State> InnerStates { get; }

            public SingleStateVertex(TState value)
            {
                State = new State(value);

                _innerStates.Add(value, State);
                InnerStates = new ReadOnlyDictionary<TState, State>(_innerStates);
            }

            public override State Update(T animatable)
            {
                return State;
            }
        }

        public class MergedStateVertex : Vertex
        {
            public Dictionary<TState, State> States { get; } = new Dictionary<TState, State>();
            public List<Predicate<T>> Predicates { get; } = new List<Predicate<T>>();
            protected internal override IReadOnlyDictionary<TState, State> InnerStates { get; }

            public MergedStateVertex()
            {
                InnerStates = new ReadOnlyDictionary<TState, State>(States);
            }

            public override State Update(T animatable)
            {
                for (int i = 0; i < Predicates.Count; i++)
                    if (Predicates[i](animatable))
                        return States.Values.ElementAt(i);

                return States.Values.Last();
            }
        }

        public class Transition : Edge<Vertex, Transition>
        {
            public Predicate<T> Predicate { get; }
            public bool OnRequest { get; set; }

            public Transition(Predicate<T> predicate)
            {
                Predicate = predicate;
            }
        }

        public interface IStateController
        {
            IBeginGoToController GoTo(TState state, Predicate<T> predicate = null);
        }

        public interface IBeginGoToController : IStateController
        {
            IBeginGoToController OnRequest();
            IGoToController Then(TState state, Predicate<T> predicate = null);
        }

        public interface IGoToController : IBeginGoToController, IAlternativeController
        {
        }

        public interface IAlternativeController : IStateController
        {
            IAlternativeController Or(TState state, Predicate<T> predicate);
        }

        public interface IBeginMetaStateController
        {
            IMetaStateController If(Predicate<T> predicate, TState state);
        }

        public interface IMetaStateController : IBeginMetaStateController
        {
            IStateController Else(TState state);
        }

        public class StateController : IStateController
        {
            private readonly AnimationGraph<T, TState> _graph;
            private readonly Vertex _state;

            public StateController(AnimationGraph<T, TState> graph, Vertex state)
            {
                _graph = graph;
                _state = state;
            }

            public IBeginGoToController GoTo(TState state, Predicate<T> predicate)
            {
                Transition transition = _graph.AddTransition(_state, _graph.ResolveVertex(state), predicate);
                return new GoToController(_graph, _state, _graph.ResolveVertex(state), transition);
            }
        }

        public class MergedStateController : IMetaStateController
        {
            private readonly AnimationGraph<T, TState> _graph;
            private readonly List<TState> _values = new List<TState>();
            private readonly List<Predicate<T>> _predicates = new List<Predicate<T>>();

            public MergedStateController(AnimationGraph<T, TState> graph)
            {
                _graph = graph;
            }

            public IMetaStateController If(Predicate<T> predicate, TState state)
            {
                _predicates.Add(predicate);
                _values.Add(state);
                return this;
            }

            public IStateController Else(TState state)
            {
                _values.Add(state);

                var metaState = new MergedStateVertex();
                metaState.States.AddRange(_values, x => x, x => new State(x));
                metaState.Predicates.AddRange(_predicates);

                foreach (TState value in _values)
                {
                    if (_graph.States.ContainsKey(value))
                        _graph.RemoveState(value);

                    _graph._states.Add(value, metaState);
                }

                _graph._graph.RegisterVertex(metaState);

                return new StateController(_graph, metaState);
            }
        }

        public class GoToController : IGoToController
        {
            private readonly AnimationGraph<T, TState> _graph;
            private readonly Vertex _vertex;
            private Vertex _currentVertex;
            private Vertex _previousVertex;
            private Transition _currentTransition;

            public GoToController(AnimationGraph<T, TState> graph, Vertex vertex, Vertex currentVertex, Transition currentTransition)
            {
                _graph = graph;
                _vertex = vertex;
                _currentVertex = currentVertex;
                _currentTransition = currentTransition;
            }

            public IBeginGoToController GoTo(TState state, Predicate<T> predicate = null)
            {
                _currentTransition = _graph.AddTransition(_vertex, _graph.ResolveVertex(state), predicate);
                return new GoToController(_graph, _vertex, _graph.ResolveVertex(state), _currentTransition);
            }

            public IBeginGoToController OnRequest()
            {
                _currentTransition.OnRequest = true;
                return this;
            }

            public IGoToController Then(TState state, Predicate<T> predicate = null)
            {
                _currentTransition = _graph.AddTransition(_currentVertex, _graph.ResolveVertex(state), predicate);
                _previousVertex = _currentVertex;
                _currentVertex = _graph.ResolveVertex(state);
                return this;
            }

            public IAlternativeController Or(TState state, Predicate<T> predicate)
            {
                _currentTransition = _graph.AddTransition(_previousVertex, _graph.ResolveVertex(state), predicate);
                _currentVertex = _graph.ResolveVertex(state);
                return this;
            }
        }
    }
}