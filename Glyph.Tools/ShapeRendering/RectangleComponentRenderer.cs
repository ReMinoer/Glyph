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

        public RectangleComponentRenderer(IShapedComponent<IRectangle> shapedComponent, Func<GraphicsDevice> graphicsDeviceFunc)
            : base(shapedComponent, new FilledRectangleSprite(graphicsDeviceFunc))
        {
            _shapedComponent = shapedComponent;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = _shapedComponent.Shape.Size / 100;
        }
    }
}