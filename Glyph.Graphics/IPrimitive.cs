using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IPrimitive
    {
        bool Visible { get; }
        IEnumerable<Vector2> Vertices { get; }
        IEnumerable<ushort> Indices { get; }
        int VertexCount { get; }
        int IndexCount { get; }
        void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex);
        void CopyToIndexArray(ushort[] indexArray, int startIndex);
        void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex);
    }
}