using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IMesh
    {
        bool Visible { get; set; }
        PrimitiveType Type { get; }
        IEnumerable<Vector2> Vertices { get; }
        IEnumerable<int> Indices { get; }
        int VertexCount { get; }
        int IndexCount { get; }
    }
}