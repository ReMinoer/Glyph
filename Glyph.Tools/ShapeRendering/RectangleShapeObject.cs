using System;
using Glyph.Composition;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class RectangleShapeRenderer : ShapeRendererBase
    {
        private readonly IShapedObject<IRectangle> _shapedObject;

        public RectangleShapeRenderer(IShapedObject<IRectangle> shapedObject, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(shapedObject.SceneNode, new FilledRectangleSprite(lazyGraphicsDevice))
        {
            _shapedObject = shapedObject;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = _shapedObject.Shape.Size / 100;
        }
    }
}