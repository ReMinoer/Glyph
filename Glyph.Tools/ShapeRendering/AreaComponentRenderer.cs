using System;
using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class AreaComponentRenderer : ShapedComponentRendererBase
    {
        public AreaComponentRenderer(IBoxedComponent boxedComponent, Func<GraphicsDevice> graphicsDeviceFunc)
            : base(boxedComponent, new FilledRectangleSprite(graphicsDeviceFunc))
        {
        }
    }
}