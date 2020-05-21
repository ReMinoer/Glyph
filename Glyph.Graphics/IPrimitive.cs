using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IPrimitive
    {
        bool Visible { get; set; }
        PrimitiveType Type { get; }
        IEnumerable<Vector2> Vertices { get; }
        IEnumerable<int> Indices { get; }
        IEnumerable<Vector2> TextureCoordinates { get; }
        int VertexCount { get; }
        int IndexCount { get; }
        void CopyToVertexArray(VertexPosition[] vertexArray, int startIndex);
        void CopyToVertexArray(VertexPositionColor[] vertexArray, int startIndex);
        void CopyToVertexArray(VertexPositionColorTexture[] vertexArray, int startIndex);
        void CopyToVertexArray(VertexPositionTexture[] vertexArray, int startIndex);
        void CopyToIndexArray(int[] indexArray, int startIndex);
    }
}