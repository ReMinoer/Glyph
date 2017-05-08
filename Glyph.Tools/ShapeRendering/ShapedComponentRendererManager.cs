using System;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Core.Tracking;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public sealed class ShapedComponentRendererManager<T> : GlyphComposite, IUpdate, IDraw
        where T : class, IBoxedComponent
    {
        public bool Visible { get; set; }
        public IFilter<IDrawClient> DrawClientFilter { get; set; } = null;
        public Color Color { get; set; }

        private readonly SceneNode _sceneNode;
        private readonly MessagingTracker<T> _tracker;
        private readonly Func<GraphicsDevice> _graphicsDeviceFunc;
        private readonly ContentLibrary _contentLibrary;
        private readonly Dictionary<T, ShapedComponentRendererBase> _colliderObjects;

        public ShapedComponentRendererManager(MessagingTracker<T> tracker, Func<GraphicsDevice> graphicsDeviceFunc, ContentLibrary contentLibrary)
        {
            Visible = true;

            Add(_sceneNode = new SceneNode());
            _tracker = tracker;
            _graphicsDeviceFunc = graphicsDeviceFunc;
            _contentLibrary = contentLibrary;
            _colliderObjects = new Dictionary<T, ShapedComponentRendererBase>();

            Color = Color.Pink * 0.5f;
            
            _tracker.Unregistered += TrackerOnUnregistered;
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (T newInstance in _tracker.NewInstances)
                AddShape(newInstance);
            _tracker.CleanNewInstances();

            foreach (ShapedComponentRendererBase colliderObject in _colliderObjects.Values)
                colliderObject.Update(elapsedTime);
        }

        public void Draw(IDrawer drawer)
        {
            if (!this.Displayed(drawer.Client))
                return;

            foreach (ShapedComponentRendererBase colliderObject in _colliderObjects.Values)
                colliderObject.Draw(drawer);
        }

        private void AddShape(T shapedObject)
        {
            var circle = shapedObject as IShapedComponent<Circle>;
            if (circle != null)
            {
                AddShape(shapedObject, new CircleComponentRenderer(circle, _graphicsDeviceFunc));
                return;
            }

            var rectangle = shapedObject as IShapedComponent<IRectangle>;
            if (rectangle != null)
            {
                AddShape(shapedObject, new RectangleComponentRenderer(rectangle, _graphicsDeviceFunc));
                return;
            }

            AddShape(shapedObject, new AreaComponentRenderer(shapedObject, _graphicsDeviceFunc));
        }

        private void AddShape(T shapedObject, ShapedComponentRendererBase shapedComponentRenderer)
        {
            shapedComponentRenderer.Color = Color;
            Add(shapedComponentRenderer);
            shapedComponentRenderer.Initialize();
            shapedComponentRenderer.LoadContent(_contentLibrary);
            _colliderObjects.Add(shapedObject, shapedComponentRenderer);
        }

        private void TrackerOnUnregistered(T shapedObject)
        {
            Remove(_colliderObjects[shapedObject]);
            _colliderObjects.Remove(shapedObject);
        }
    }
}