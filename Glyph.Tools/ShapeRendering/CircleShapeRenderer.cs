using System;
using Glyph.Composition;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class CircleShapeRenderer : ShapeRendererBase
    {
        private readonly IShapedObject<ICircle> _shapedObject;

        public CircleShapeRenderer(IShapedObject<ICircle> shapedObject, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(shapedObject.SceneNode, new CircleSprite(lazyGraphicsDevice))
        {
            _shapedObject = shapedObject;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = new Vector2(_shapedObject.Shape.Radius, _shapedObject.Shape.Radius) / 100;
        }
    }
}