using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IPrimitive
    {
        IReadOnlyCollection<Vector2> Vertices { get; }
        void CopyToArray(VertexPositionColor[] vertexArray, int startIndex);
        void DrawPrimitives(GraphicsDevice graphicsDevice, int startIndex);
    }
}