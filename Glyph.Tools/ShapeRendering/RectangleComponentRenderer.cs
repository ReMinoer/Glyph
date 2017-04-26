using System;
using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class RectangleComponentRenderer : ShapedComponentRendererBase
    {
        public RectangleComponentRenderer(IShapedComponent<IRectangle> shapedComponent, Func<GraphicsDevice> graphicsDeviceFunc)
            : base(shapedComponent, new FilledRectangleSprite(graphicsDeviceFunc))
        {
        }
    }
}