using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class PrimitiveComponent : GlyphComponent, IBoxedComponent, IPrimitive
    {
        public bool Visible { get; set; } = true;
        public IPrimitive Primitive { get; set; }
        public IArea Area => MathUtils.GetBoundingBox(Primitive.Vertices);

        public IEnumerable<Vector2> Vertices => Primitive?.Vertices ?? Enumerable.Empty<Vector2>();
        public IEnumerable<ushort> Indices => Primitive?.Indices ?? Enumerable.Empty<ushort>();
        public int VertexCount => Primitive?.VertexCount ?? 0;
        public int IndexCount => Primitive?.IndexCount ?? 0;
        public void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex) => Primitive?.CopyToVertexArray(vertexArray, startIndex);
        public void CopyToIndexArray(ushort[] indexArray, int startIndex) => Primitive?.CopyToIndexArray(indexArray, startIndex);
        public void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex) => Primitive?.DrawPrimitives(graphicsDevice, verticesIndex, indicesIndex);
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