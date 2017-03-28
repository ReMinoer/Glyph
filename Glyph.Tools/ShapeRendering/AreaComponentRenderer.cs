using System;
using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Tools.ShapeRendering
{
    public class AreaComponentRenderer : ShapedComponentRendererBase
    {
        private readonly IBoxedComponent _boxedComponent;

        public AreaComponentRenderer(IBoxedComponent boxedComponent, Func<GraphicsDevice> graphicsDeviceFunc)
            : base(boxedComponent, new FilledRectangleSprite(graphicsDeviceFunc))
        {
            _boxedComponent = boxedComponent;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = _boxedComponent.Area.BoundingBox.Size / 100;
        }
    }
}