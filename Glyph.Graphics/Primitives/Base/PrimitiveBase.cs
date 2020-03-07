using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public abstract class PrimitiveBase : IPrimitive
    {
        public abstract IReadOnlyCollection<Vector2> Vertices { get; }
        public abstract IReadOnlyCollection<ushort> Indices { get; }
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
        
        public abstract void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex);
    }
}