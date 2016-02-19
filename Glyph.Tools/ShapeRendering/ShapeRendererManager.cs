using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Tracking;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class ShapeRendererManager<T> : GlyphComposite<ShapeRendererBase>, IUpdate, IDraw
        where T : class, IShapedObject
    {
        public bool Visible { get; set; }
        private readonly MessagingTracker<T> _tracker;
        private readonly Lazy<GraphicsDevice> _lazyGraphicsDevice;
        private readonly ContentLibrary _contentLibrary;
        private readonly Dictionary<T, ShapeRendererBase> _colliderObjects;

        public ShapeRendererManager(MessagingTracker<T> tracker, Lazy<GraphicsDevice> lazyGraphicsDevice, ContentLibrary contentLibrary)
        {
            _tracker = tracker;
            _lazyGraphicsDevice = lazyGraphicsDevice;
            _contentLibrary = contentLibrary;
            _colliderObjects = new Dictionary<T, ShapeRendererBase>();

            _tracker.Registered += TrackerOnRegistered;
            _tracker.Unregistered += TrackerOnUnregistered;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            foreach (ShapeRendererBase colliderObject in this)
                colliderObject.Update(elapsedTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (ShapeRendererBase colliderObject in this)
                colliderObject.Draw(spriteBatch);
        }

        private void TrackerOnRegistered(T shapedObject)
        {
            var rectangle = shapedObject as IShapedObject<IRectangle>;
            if (rectangle != null)
            {
                var shapeRenderer = new RectangleShapeRenderer(rectangle, _lazyGraphicsDevice);
                shapeRenderer.LoadContent(_contentLibrary);
                Add(shapeRenderer);
                _colliderObjects.Add(shapedObject, shapeRenderer);
                return;
            }

            var circle = shapedObject as IShapedObject<ICircle>;
            if (circle != null)
            {
                var shapeRenderer = new CircleShapeRenderer(circle, _lazyGraphicsDevice);
                shapeRenderer.LoadContent(_contentLibrary);
                Add(shapeRenderer);
                _colliderObjects.Add(shapedObject, shapeRenderer);
                return;
            }

            throw new NotSupportedException();
        }

        private void TrackerOnUnregistered(T shapedObject)
        {
            Remove(_colliderObjects[shapedObject]);
            _colliderObjects.Remove(shapedObject);
        }
    }
}