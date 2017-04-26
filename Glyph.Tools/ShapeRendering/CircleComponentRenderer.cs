using System;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class CircleComponentRenderer : ShapedComponentRendererBase
    {
        public CircleComponentRenderer(IShapedComponent<Circle> shapedComponent, Func<GraphicsDevice> graphicsDeviceFunc)
            : base(shapedComponent, new CircleSprite(graphicsDeviceFunc))
        {
        }
    }
}