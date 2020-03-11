using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives.Base
{
    public abstract class PrimitiveBase : IPrimitive
    {
        public bool Visible { get; set; } = true;
        public abstract PrimitiveType PrimitiveType { get; }
        public abstract IEnumerable<Vector2> Vertices { get; }
        public abstract IEnumerable<ushort> Indices { get; }
        public abstract int VertexCount { get; }
        public abstract int IndexCount { get; }
        public abstract Color[] Colors { get; set; }

        public void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            int i = 0;
            foreach (Vector2 vertex in Vertices)
            {
                vertexArray[startIndex + i] = new VertexPositionColor(vertex.ToVector3(), GetVertexColor(i));
                i++;
            }
        }
        protected virtual Color GetVertexColor(int i) => Colors[i % Colors.Length];

        public void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            if (IndexCount > 0)
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType, verticesIndex, indicesIndex, GetPrimitiveCount(PrimitiveType, IndexCount));
            else
                graphicsDevice.DrawPrimitives(PrimitiveType, verticesIndex, GetPrimitiveCount(PrimitiveType, VertexCount));
        }

        private int GetPrimitiveCount(PrimitiveType primitiveType, int vertexCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.TriangleList: return vertexCount / 3;
                case PrimitiveType.TriangleStrip: return vertexCount - 2;
                case PrimitiveType.LineList: return vertexCount / 2;
                case PrimitiveType.LineStrip: return vertexCount - 1;
                default: throw new NotSupportedException();
            }
        }
    }
}