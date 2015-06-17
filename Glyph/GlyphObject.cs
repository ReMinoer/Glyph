using System;
using System.Collections.Generic;
using Diese.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class GlyphObject : Composite<IGlyphComponent>, IGlyphObject
    {
        // TODO : Handle singletons & single components
        private bool _enabled;
        private bool _visible;

        private readonly DependencyGraph<IUpdate> _updateDependencies;
        private readonly DependencyGraph<IDraw> _drawDependencies;
        private List<IUpdate> _orderedUpdate;
        private List<IDraw> _orderedDraw;
        private readonly List<ILoadContent> _cachedLoadContent;
        private readonly List<IHandleInput> _cachedHandleInput;

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

        public GlyphObject()
        {
            _updateDependencies = new DependencyGraph<IUpdate>();
            _drawDependencies = new DependencyGraph<IDraw>();

            _cachedLoadContent = new List<ILoadContent>();
            _orderedUpdate = new List<IUpdate>();
            _cachedHandleInput = new List<IHandleInput>();
            _orderedDraw = new List<IDraw>();
        }

        public void Initialize()
        {
            foreach (IGlyphComponent component in Components)
                component.Initialize();

            MyInitialize();

            _updateDependencies.GraphEdited += UpdateDependenciesOnGraphEdited;
            _drawDependencies.GraphEdited += DrawDependenciesOnGraphEdited;
        }

        protected virtual void MyInitialize()
        {
        }

        public void LoadContent(ContentLibrary contentLibrary)
        {
            foreach (ILoadContent component in _cachedLoadContent)
                component.LoadContent(contentLibrary);

            MyLoadContent(contentLibrary);
        }

        protected virtual void MyLoadContent(ContentLibrary contentLibrary)
        {
        }

        public void Update()
        {
            if (!Enabled)
                return;

            MyUpdate();

            foreach (IUpdate component in _orderedUpdate)
                component.Update();
        }

        protected virtual void MyUpdate()
        {
        }

        public void HandleInput()
        {
            if (!Enabled)
                return;

            MyHandleInput();

            foreach (IHandleInput component in _cachedHandleInput)
                component.HandleInput();
        }

        protected virtual void MyHandleInput()
        {
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (!Visible)
                return;

            MyDraw(spriteBatch, graphicsDevice);

            foreach (IDraw component in _orderedDraw)
                component.Draw(spriteBatch, graphicsDevice);
        }

        protected virtual void MyDraw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void Dispose()
        {
            foreach (IGlyphComponent component in Components)
                component.Dispose();

            Clear();
        }

        public override void Add(IGlyphComponent item)
        {
            AddComponentToCache(item);
            base.Add(item);
        }

        public override void Insert(int index, IGlyphComponent item)
        {
            AddComponentToCache(item);
            base.Insert(index, item);
        }

        public override void Clear()
        {
            foreach (IGlyphComponent component in this)
                RemoveComponentFromCache(component);
            base.Clear();
        }

        public override bool Remove(IGlyphComponent item)
        {
            RemoveComponentFromCache(item);
            return base.Remove(item);
        }

        public override void RemoveAt(int index)
        {
            RemoveComponentFromCache(this[index]);
            base.RemoveAt(index);
        }

        protected void AddUpdateDependency(IUpdate dependent, IUpdate dependency)
        {
            _updateDependencies.AddDependency(dependent, dependency);
        }

        protected void RemoveUpdateDependency(IUpdate dependent, IUpdate dependency)
        {
            _updateDependencies.RemoveDependency(dependent, dependency);
        }

        protected void AddDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.AddDependency(dependent, dependency);
        }

        protected void RemoveDrawDependency(IDraw dependent, IDraw dependency)
        {
            _drawDependencies.RemoveDependency(dependent, dependency);
        }

        private void AddComponentToCache(IGlyphComponent component)
        {
            if (Contains(component))
                throw new ArgumentException("Component provided is already contained by this entity !");
            if (GetComponentInParents(component.GetType()) != null)
                throw new ArgumentException("Component provided cannot become a child from itself !");

            var update = component as IUpdate;
            if (update != null)
                _updateDependencies.AddItem(update);

            var draw = component as IDraw;
            if (draw != null)
                _drawDependencies.AddItem(draw);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _cachedLoadContent.Add(loadContent);

            var handleInput = component as IHandleInput;
            if (loadContent != null)
                _cachedHandleInput.Add(handleInput);
        }

        private void RemoveComponentFromCache(IGlyphComponent component)
        {
            if (!Contains(component))
                throw new ArgumentException("Component provided is not contained by this entity !");

            var update = component as IUpdate;
            if (update != null)
                _updateDependencies.RemoveItem(update);

            var draw = component as IDraw;
            if (draw != null)
                _drawDependencies.RemoveItem(draw);

            var loadContent = component as ILoadContent;
            if (loadContent != null)
                _cachedLoadContent.Remove(loadContent);

            var handleInput = component as IHandleInput;
            if (loadContent != null)
                _cachedHandleInput.Remove(handleInput);
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