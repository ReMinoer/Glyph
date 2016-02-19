using System;
using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Composition.Layers;
using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Messaging;
using Glyph.Physics.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Scripting.Triggers
{
    public class TriggerArea : GlyphContainer, IEnableable, ILoadContent, IDraw, ITriggerArea<OriginRectangle>
    {
        private readonly ILayerManager _layerManager;
        private readonly IRouter<OnEnterTrigger> _router;
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

        public OriginRectangle Shape
        {
            get { return new OriginRectangle(_sceneNode.Position, Size); }
        }

        ISceneNode IShapedObject.SceneNode
        {
            get { return new ReadOnlySceneNode(_sceneNode); }
        }

        IShape IShapedObject.Shape
        {
            get { return Shape; }
        }

        Vector2 IShape.Center
        {
            get { return Shape.Center; }
            set { _sceneNode.Position = value - Size / 2; }
        }

        event Action<ITrigger> ITrigger.Triggered
        {
            add { _triggerVar.Triggered += value; }
            remove { _triggerVar.Triggered -= value; }
        }

        public TriggerArea(bool singleUse, Lazy<GraphicsDevice> graphicsDevice, ILayerManager layerManager, IRouter<OnEnterTrigger> router)
        {
            _layerManager = layerManager;
            _router = router;
            _triggerVar = new TriggerVar(singleUse);

            Components.Add(_sceneNode = new SceneNode());
            Components.Add(_rectangleSprite = new RectangleSprite(graphicsDevice));
            Components.Add(_spriteTransformer = new SpriteTransformer());
            Components.Add(_spriteRenderer = new SpriteRenderer(_rectangleSprite, _sceneNode));
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
                _triggerVar.SetActive(false);

            if (!Enabled || SingleUse)
                return;

            foreach (IActor actor in actors)
            {
                if (_parentLayer != null && actor.LayerRoot != null && actor.LayerRoot.Layer.Index != _parentLayer.Layer.Index)
                    continue;

                foreach (ICollider collider in actor.Colliders)
                    if (collider.Intersects(Shape))
                    {
                        _triggerVar.Active = true;
                        _router.Send(new OnEnterTrigger(this, actor));

                        return;
                    }
            }
        }

        public bool Intersects(IRectangle rectangle)
        {
            return Shape.Intersects(rectangle);
        }

        public bool Intersects(ICircle circle)
        {
            return Shape.Intersects(circle);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Shape.ContainsPoint(point);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Name, Shape, Active);
        }

        private void RefreshScale()
        {
            _spriteTransformer.Scale = Size / 100;
        }
    }
}