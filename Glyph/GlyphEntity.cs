using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Composition;
using Glyph.Exceptions;

namespace Glyph
{
    public class GlyphEntity : Composite<IGlyphComponent, GlyphEntity>, IGlyphComposite, ILoadContent, IHandleInput, IDraw, IDependencyProvider<IGlyphComponent>
    {
        // TODO : Handle singletons & single components
        static private readonly Stack<Type> DependencyStack = new Stack<Type>();

        private bool _enabled;
        private bool _visible;

        private readonly IDependencyGraph<IUpdate> _updateDependencies;
        private readonly IDependencyGraph<IDraw> _drawDependencies;
        private List<IUpdate> _orderedUpdate;
        private List<IDraw> _orderedDraw;
        private readonly List<ILoadContent> _orderedLoadContent;
        private readonly List<IHandleInput> _orderedHandleInput;

        public virtual bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value)
                    return;

                if (EnabledChanged != null)
                    EnabledChanged(this, EventArgs.Empty);

                _enabled = value;
            }
        }

        public virtual bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                if (VisibleChanged != null)
                    VisibleChanged(this, EventArgs.Empty);

                _visible = value;
            }
        }

        public event EventHandler EnabledChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler UpdateOrderChanged;
        public event EventHandler DrawOrderChanged;

        public GlyphEntity()
        {
            _updateDependencies = new DependencyGraph<IUpdate>();
            _drawDependencies = new DependencyGraph<IDraw>();
            _orderedUpdate = new List<IUpdate>();
            _orderedDraw = new List<IDraw>();
            _orderedLoadContent = new List<ILoadContent>();
            _orderedHandleInput = new List<IHandleInput>();

            _updateDependencies.GraphEdited += UpdateDependenciesOnGraphEdited;
            _drawDependencies.GraphEdited += DrawDependenciesOnGraphEdited;
        }

        public void Initialize()
        {
            InitializeLocal();

            foreach (IGlyphComponent component in Components)
                component.Initialize();
        }

        protected virtual void InitializeLocal()
        {
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (ILoadContent component in _orderedLoadContent)
                component.LoadContent(contentLibrary);

            LoadContentLocal(contentLibrary);
        }

        protected virtual void LoadContentLocal(ContentLibrary contentLibrary)
        {
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            UpdateLocal(elapsedTime);

            foreach (IUpdate component in _orderedUpdate)
                component.Update(elapsedTime);
        }

        protected virtual void UpdateLocal(ElapsedTime elapsedTime)
        {
        }

        public void HandleInput()
        {
            if (!Enabled)
                return;

            HandleInputLocal();

            foreach (IHandleInput component in _orderedHandleInput)
                component.HandleInput();
        }

        protected virtual void HandleInputLocal()
        {
        }

        public void Draw()
        {
            if (!Visible)
                return;

            DrawLocal();

            foreach (IDraw component in _orderedDraw)
                component.Draw();
        }

        protected virtual void DrawLocal()
        {
        }

        public virtual void Dispose()
        {
            foreach (IGlyphComponent component in Components)
                component.Dispose();

            Clear();
        }

        public T Add<T>()
            where T : class, IGlyphComponent, new()
        {
            var component = new T();
            Add(component);

            return component;
        }

        public IGlyphComponent Add(Type componentType)
        {
            var component = Activator.CreateInstance(componentType) as IGlyphComponent;
            Add(component);

            return component;
        }

        public override sealed void Add(IGlyphComponent item)
        {
            if (Contains(item))
                throw new ArgumentException("Component provided is already contained by this entity !");

            Type type = item.GetType();

            if (GetComponent(type) != null && type.GetCustomAttributes(typeof(SinglePerParentAttribute)).Any())
                throw new SingleComponentException(type);

            var dependent = item as IDependent<IGlyphComponent>;
            if (dependent != null)
                dependent.BindDependencies(this);

            base.Add(item);
            AddComponentToCache(item);
        }

        public override sealed void Clear()
        {
            foreach (IGlyphComponent component in this)
                RemoveComponentFromCache(component);

            base.Clear();
        }

        public override sealed bool Remove(IGlyphComponent item)
        {
            if (!Contains(item))
                throw new ArgumentException("Component provided is not contained by this entity !");

            RemoveComponentFromCache(item);
            return base.Remove(item);
        }

        public T Resolve<T>()
            where T : class, IGlyphComponent, new()
        {
            Type type = typeof(T);

            if (DependencyStack.Contains(type))
                throw new CyclicDependencyException(DependencyStack);

            var dependency = GetComponent<T>();
            if (dependency == null)
            {
                DependencyStack.Push(type);
                dependency = Add<T>();
                DependencyStack.Pop();
            }

            return dependency;
        }

        protected internal void AddUpdateDependency(IUpdate dependent, IUpdate dependency)
        {
            _updateDependencies.AddDependency(dependent, dependency);
        }

        protected internal void RemoveUpdateDependency(IUpdate dependent, IUpdate dependency)
        {
            _updateDependencies.RemoveDependency(dependent, dependency);
        }

        protected internal void AddDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.AddDependency(dependent, dependency);
        }

        protected internal void RemoveDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.RemoveDependency(dependent, dependency);
        }

        private void AddComponentToCache(IGlyphComponent component)
        {
            var update = component as IUpdate;
            if (update != null)
                _updateDependencies.AddItem(update);

            var draw = component as IDraw;
            if (draw != null)
                _drawDependencies.AddItem(draw);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _orderedLoadContent.Add(loadContent);

            var handleInput = component as IHandleInput;
            if (loadContent != null)
                _orderedHandleInput.Add(handleInput);
        }

        private void RemoveComponentFromCache(IGlyphComponent component)
        {
            var update = component as IUpdate;
            if (update != null)
                _updateDependencies.RemoveItem(update);

            var draw = component as IDraw;
            if (draw != null)
                _drawDependencies.RemoveItem(draw);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _orderedLoadContent.Remove(loadContent);

            var handleInput = component as IHandleInput;
            if (loadContent != null)
                _orderedHandleInput.Remove(handleInput);
        }

        private void UpdateDependenciesOnGraphEdited(object sender, EventArgs eventArgs)
        {
            _orderedUpdate = _updateDependencies.GetTopologicalOrder();

            if (UpdateOrderChanged != null)
                UpdateOrderChanged.Invoke(this, EventArgs.Empty);
        }

        private void DrawDependenciesOnGraphEdited(object sender, EventArgs eventArgs)
        {
            _orderedDraw = _drawDependencies.GetTopologicalOrder();

            if (DrawOrderChanged != null)
                DrawOrderChanged.Invoke(this, EventArgs.Empty);
        }
    }
}