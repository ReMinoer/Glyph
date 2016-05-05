using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Tracking;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public sealed class ShapeRendererManager<T> : GlyphComposite, IUpdate, IDraw
        where T : class, IShapedObject
    {
        public bool Visible { get; set; }
        public Color Color { get; set; }

        private readonly SceneNode _sceneNode;
        private readonly MessagingTracker<T> _tracker;
        private readonly Lazy<GraphicsDevice> _lazyGraphicsDevice;
        private readonly ContentLibrary _contentLibrary;
        private readonly Dictionary<T, ShapeRendererBase> _colliderObjects;

        public ShapeRendererManager(MessagingTracker<T> tracker, Lazy<GraphicsDevice> lazyGraphicsDevice, ContentLibrary contentLibrary)
        {
            Visible = true;

            Add(_sceneNode = new SceneNode());
            _tracker = tracker;
            _lazyGraphicsDevice = lazyGraphicsDevice;
            _contentLibrary = contentLibrary;
            _colliderObjects = new Dictionary<T, ShapeRendererBase>();

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

            foreach (ShapeRendererBase colliderObject in _colliderObjects.Values)
                colliderObject.Update(elapsedTime);
        }

        public void Draw(IDrawer drawer)
        {
            if (!Visible)
                return;

            foreach (ShapeRendererBase colliderObject in _colliderObjects.Values)
                colliderObject.Draw(drawer);
        }

        private void AddShape(T shapedObject)
        {
            var rectangle = shapedObject as IShapedObject<IRectangle>;
            if (rectangle != null)
            {
                AddShape(shapedObject, new RectangleShapeRenderer(rectangle, _lazyGraphicsDevice));
                return;
            }

            var circle = shapedObject as IShapedObject<ICircle>;
            if (circle != null)
            {
                AddShape(shapedObject, new CircleShapeRenderer(circle, _lazyGraphicsDevice));
                return;
            }

            throw new NotSupportedException();
        }

        private void AddShape(T shapedObject, ShapeRendererBase shapeRenderer)
        {
            shapeRenderer.Color = Color;
            Add(shapeRenderer);
            shapeRenderer.Initialize();
            shapeRenderer.LoadContent(_contentLibrary);
            _colliderObjects.Add(shapedObject, shapeRenderer);
        }

        private void TrackerOnUnregistered(T shapedObject)
        {
            Remove(_colliderObjects[shapedObject]);
            _colliderObjects.Remove(shapedObject);
        }
    }
}