using System.Collections.Generic;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class PrimitiveComponent : GlyphComponent, IBoxedComponent, IPrimitive
    {
        public IPrimitive Primitive { get; set; }
        public IArea Area => MathUtils.GetBoundingBox(Primitive.Vertices);

        public IReadOnlyCollection<Vector2> Vertices => Primitive?.Vertices ?? new Vector2[0];
        public void CopyToArray(VertexPositionColor[] vertexArray, int startIndex) => Primitive?.CopyToArray(vertexArray, startIndex);
        public void DrawPrimitives(GraphicsDevice graphicsDevice, int startIndex) => Primitive?.DrawPrimitives(graphicsDevice, startIndex);
    }

    public class PrimitiveComponent<TPrimitive> : PrimitiveComponent
        where TPrimitive : IPrimitive
    {
        private TPrimitive _primitive;

        new public TPrimitive Primitive
        {
            get => _primitive;
            set
            {
                _primitive = value;
                base.Primitive = value;
            }
        }
    }
}