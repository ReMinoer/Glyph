using System;
using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class RectangleComponentRenderer : ShapedComponentRendererBase
    {
        private readonly IShapedComponent<IRectangle> _shapedComponent;

        public RectangleComponentRenderer(IShapedComponent<IRectangle> shapedComponent, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(shapedComponent, new FilledRectangleSprite(lazyGraphicsDevice))
        {
            _shapedComponent = shapedComponent;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = _shapedComponent.Shape.Size / 100;
        }
    }
}