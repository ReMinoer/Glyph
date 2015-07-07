using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Diese.Injection;
using Glyph.Exceptions;

namespace Glyph
{
    public class GlyphObject : GlyphComposite, IEnableable, ILoadContent, IUpdate, IHandleInput, IDraw
    {
        protected readonly DelegatedComponent This;

        private readonly GlyphInjector _injector;

        private readonly IDependencyGraph<IGlyphComponent> _logicalDependencies;
        private readonly IDependencyGraph<IDraw> _drawDependencies;
        private ICollection<IGlyphComponent> _orderedInitialize;
        private ICollection<ILoadContent> _orderedLoadContent;
        private ICollection<IUpdate> _orderedUpdate;
        private ICollection<IHandleInput> _orderedHandleInput;
        private ICollection<IDraw> _orderedDraw;

        public virtual bool Enabled { get; set; }
        public virtual bool Visible { get; set; }

        public GlyphObject(IDependencyRegistry dependencyRegistry)
        {
            This = new DelegatedComponent(this);

            _injector = new GlyphInjector(this, dependencyRegistry);

            _logicalDependencies = new DependencyGraph<IGlyphComponent>();
            _logicalDependencies.AddItem(This);
            _drawDependencies = new DependencyGraph<IDraw>();
            _drawDependencies.AddItem(This);

            _orderedInitialize = new List<IGlyphComponent> { This };
            _orderedLoadContent = new List<ILoadContent> { This };
            _orderedUpdate = new List<IUpdate> { This };
            _orderedHandleInput = new List<IHandleInput> { This };
            _orderedDraw = new List<IDraw> { This };
        }

        public override void Initialize()
        {
            foreach (IGlyphComponent component in Components)
                component.Initialize();
        }

        protected virtual void LocalInitialize()
        {
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (ILoadContent component in _orderedLoadContent)
                component.LoadContent(contentLibrary);
        }

        protected virtual void LocalLoadContent(ContentLibrary contentLibrary)
        {
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled)
                return;

            foreach (IUpdate component in _orderedUpdate)
                component.Update(elapsedTime);
        }

        protected virtual void LocalUpdate(ElapsedTime elapsedTime)
        {
        }

        public void HandleInput()
        {
            if (!Enabled)
                return;

            foreach (IHandleInput component in _orderedHandleInput)
                component.HandleInput();
        }

        protected virtual void LocalHandleInput()
        {
        }

        public void Draw()
        {
            if (!Visible)
                return;

            foreach (IDraw component in _orderedDraw)
                component.Draw();
        }

        protected virtual void LocalDraw()
        {
        }

        public override void Dispose()
        {
            foreach (IGlyphComponent component in Components)
                component.Dispose();
        }

        public T Add<T>()
            where T : class, IGlyphComponent, new()
        {
            var component = _injector.Resolve<T>();
            Add(component);

            return component;
        }

        public IGlyphComponent Add(Type componentType)
        {
            var component = _injector.Resolve(componentType) as IGlyphComponent;
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

            bool valid = base.Remove(item);

            if (!valid)
                return false;

            RemoveComponentFromCache(item);
            return true;
        }

        protected void AddLogicalDependency(IGlyphComponent dependent, IGlyphComponent dependency)
        {
            _logicalDependencies.AddDependency(dependent, dependency);
            RefreshLogicalOrder();
        }

        protected void RemoveLogicalDependency(IGlyphComponent dependent, IGlyphComponent dependency)
        {
            _logicalDependencies.RemoveDependency(dependent, dependency);
        }

        protected void AddDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.AddDependency(dependent, dependency);
            RefreshDrawOrder();
        }

        protected void RemoveDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.RemoveDependency(dependent, dependency);
        }

        private void AddComponentToCache(IGlyphComponent component)
        {
            _logicalDependencies.AddItem(component);
            _orderedInitialize.Add(component);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _orderedLoadContent.Add(loadContent);

            var update = component as IUpdate;
            if (update != null)
                _orderedUpdate.Add(update);

            var handleInput = component as IHandleInput;
            if (handleInput != null)
                _orderedHandleInput.Add(handleInput);

            var draw = component as IDraw;
            if (draw != null)
            {
                _drawDependencies.AddItem(draw);
                _orderedDraw.Add(draw);
            }
        }

        private void RemoveComponentFromCache(IGlyphComponent component)
        {
            _logicalDependencies.RemoveItem(component);
            _orderedInitialize.Remove(component);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _orderedLoadContent.Remove(loadContent);

            var update = component as IUpdate;
            if (update != null)
                _orderedUpdate.Remove(update);

            var handleInput = component as IHandleInput;
            if (handleInput != null)
                _orderedHandleInput.Remove(handleInput);

            var draw = component as IDraw;
            if (draw != null)
            {
                _drawDependencies.RemoveItem(draw);
                _orderedDraw.Remove(draw);
            }
        }

        private void RefreshLogicalOrder()
        {
            IList<IGlyphComponent> topologicalOrder = _logicalDependencies.GetTopologicalOrder().ToList();

            if (_orderedInitialize.SequenceEqual(topologicalOrder))
                return;

            _orderedInitialize = topologicalOrder;
            _orderedLoadContent = topologicalOrder.Where(x => x is ILoadContent).Cast<ILoadContent>().ToList();
            _orderedUpdate = topologicalOrder.Where(x => x is IUpdate).Cast<IUpdate>().ToList();
            _orderedHandleInput = topologicalOrder.Where(x => x is IHandleInput).Cast<IHandleInput>().ToList();
        }

        private void RefreshDrawOrder()
        {
            _orderedDraw = _drawDependencies.GetTopologicalOrder().ToList();
        }

        protected class DelegatedComponent : GlyphComponent, IEnableable, ILoadContent, IUpdate, IHandleInput, IDraw
        {
            private readonly GlyphObject _baseComponent;

            public bool Enabled
            {
                get { return true; }
            }

            public bool Visible
            {
                get { return true; }
            }

            internal DelegatedComponent(GlyphObject baseComponent)
            {
                _baseComponent = baseComponent;
            }

            public override sealed void Initialize()
            {
                _baseComponent.LocalInitialize();
            }

            public void LoadContent(ContentLibrary contentLibrary)
            {
                _baseComponent.LocalLoadContent(contentLibrary);
            }

            public void Update(ElapsedTime elapsedTime)
            {
                _baseComponent.LocalUpdate(elapsedTime);
            }

            public void HandleInput()
            {
                _baseComponent.LocalHandleInput();
            }

            public void Draw()
            {
                _baseComponent.LocalDraw();
            }
        }
    }
}