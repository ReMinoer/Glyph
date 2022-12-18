using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface IVisualMesh : IMesh
    {
        IEnumerable<IVisualMeshPart> Parts { get; }
        IEnumerable<Vector2> TextureCoordinates { get; }
        void CopyToVertexArray(VertexPositionColorTexture[] vertexArray, int startIndex);
        void CopyToIndexArray(int[] indexArray, int startIndex);
    }

    public interface IVisualMeshPart
    {
        Effect Effect { get; }
        IEffectMatrices EffectMatrices { get; }
        int VertexCount { get; }
        int TriangulationIndexCount { get; }
    }
}