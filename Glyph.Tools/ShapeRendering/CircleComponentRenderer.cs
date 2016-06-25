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
        private readonly IShapedComponent<ICircle> _shapedComponent;

        public CircleComponentRenderer(IShapedComponent<ICircle> shapedComponent, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(shapedComponent, new CircleSprite(lazyGraphicsDevice))
        {
            _shapedComponent = shapedComponent;
        }

        protected override sealed void UpdateSize()
        {
            SpriteTransformer.Scale = new Vector2(_shapedComponent.Shape.Radius, _shapedComponent.Shape.Radius) / 100;
        }
    }
}