using System;
using System.Collections.Generic;
using Glyph.Animation;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Physics.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Scripting.Triggers
{
    public class TriggerArea : GlyphContainer, IEnableable, ILoadContent, IDraw, ITriggerArea
    {
        private readonly ILayerManager _layerManager;
        private readonly TriggerVar _triggerVar;
        private readonly SceneNode _sceneNode;
        private readonly RectangleSprite _rectangleSprite;
        private readonly SpriteTransformer _spriteTransformer;
        private readonly SpriteRenderer _spriteRenderer;
        private ILayerRoot _parentLayer;
        private Vector2 _size;
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public bool Active
        {
            get { return _triggerVar.Active; }
        }

        public bool SingleUse
        {
            get { return _triggerVar.SingleUse; }
        }

        public Vector2 LocalPosition
        {
            get { return _sceneNode.LocalPosition; }
            set { _sceneNode.LocalPosition = value; }
        }

        public Vector2 Size
        {
            get { return _size; }
            set
            {
                _size = value;
                RefreshScale();
            }
        }

        public OriginRectangle Bounds
        {
            get { return new OriginRectangle(_sceneNode.Position, Size); }
        }

        Vector2 IShape.Center
        {
            get { return Bounds.Center; }
            set { _sceneNode.Position = value - Size / 2; }
        }

        public event TriggerAreaEventHandler Triggered;

        event TriggerEventHandler ITrigger.Triggered
        {
            add { _triggerVar.Triggered += value; }
            remove { _triggerVar.Triggered -= value; }
        }

        public TriggerArea(bool singleUse, Lazy<GraphicsDevice> graphicsDevice, ILayerManager layerManager)
            : base(4)
        {
            _layerManager = layerManager;
            _triggerVar = new TriggerVar(singleUse);

            Components[0] = _sceneNode = new SceneNode();
            Components[1] = _rectangleSprite = new RectangleSprite(graphicsDevice);
            Components[2] = _spriteTransformer = new SpriteTransformer();
            Components[3] = _spriteRenderer = new SpriteRenderer(_rectangleSprite, _sceneNode);
        }

        public override void Initialize()
        {
            base.Initialize();

            // TODO : Handle layer root change at runtime
            if (_layerManager != null)
                _parentLayer = _layerManager.GetLayerRoot(this);
        }

        public void LoadContent(ContentLibrary ressources)
        {
            _rectangleSprite.LoadContent(ressources);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            _spriteRenderer.Draw(spriteBatch);
        }

        public void UpdateStatus(IEnumerable<IActor> actors)
        {
            if (!_triggerVar.SingleUse)
                _triggerVar.Active = false;

            if (!Enabled || SingleUse)
                return;

            foreach (IActor actor in actors)
            {
                if (_parentLayer != null && actor.LayerRoot != null && actor.LayerRoot.Layer.Index != _parentLayer.Layer.Index)
                    continue;

                foreach (ICollider collider in actor.Colliders)
                    if (collider.Intersects(Bounds))
                    {
                        _triggerVar.Active = true;

                        if (Triggered != null)
                            Triggered.Invoke(this, actor);

                        return;
                    }
            }
        }

        public bool Intersects(IRectangle rectangle)
        {
            return Bounds.Intersects(rectangle);
        }

        public bool Intersects(ICircle circle)
        {
            return Bounds.Intersects(circle);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Bounds.ContainsPoint(point);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Name, Bounds, Active);
        }

        private void RefreshScale()
        {
            _spriteTransformer.Scale = Size / 100;
        }
    }
}