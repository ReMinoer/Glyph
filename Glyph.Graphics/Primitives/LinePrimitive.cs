using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class LinePrimitive : IPrimitive
    {
        public bool Visible { get; set; } = true;
        public Vector2[] Vertices { get; set; }
        public Color[] Colors { get; set; }
        public VertexBufferType BufferType { get; set; } = VertexBufferType.Strip;

        IEnumerable<Vector2> IPrimitive.Vertices => Vertices;
        IEnumerable<ushort> IPrimitive.Indices => null;
        int IPrimitive.VertexCount => Vertices.Length;
        int IPrimitive.IndexCount => 0;

        public LinePrimitive()
        {
        }

        public LinePrimitive(params Vector2[] vertices)
        {
            Vertices = vertices;
        }

        public LinePrimitive(Color color, params Vector2[] vertices)
            : this(vertices)
        {
            Colors = new[] { color };
        }

        public LinePrimitive(Color color, VertexBufferType bufferType, params Vector2[] vertices)
            : this(color, vertices)
        {
            BufferType = bufferType;
        }

        public void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            for (int i = 0; i < Vertices.Length; i++)
                vertexArray[startIndex + i] = new VertexPositionColor(Vertices[i].ToVector3(), Colors[i % Colors.Length]);
        }

        public void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            switch (BufferType)
            {
                case VertexBufferType.List:
                    graphicsDevice.DrawPrimitives(PrimitiveType.LineList, verticesIndex, Vertices.Length / 2);
                    break;
                case VertexBufferType.Strip:
                    graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, verticesIndex, Vertices.Length - 1);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}