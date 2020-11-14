using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.Meshes.Utils;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.Utils;

namespace Glyph.Graphics.Meshes
{
    public class GridMesh<T> : ProceduralMeshBase
    {
        public IGrid<T> Grid { get; set; }
        public Func<int, int, T, bool> MeshingBehavior { get; set; }

        public override PrimitiveType Type => PrimitiveType.TriangleList;
        protected override Color GetColor(int vertexIndex) => Color.White;

        protected override IEnumerable<Vector2> GetRefreshedVertices()
        {
            if (Grid == null)
                yield break;

            var vertexRange = new IndexRange(Grid.Lengths().Select(x => x + 1).ToArray());

            int[] indexes = vertexRange.GetResetIndex();
            while (vertexRange.MoveIndex(indexes))
                yield return Grid.ToWorldPoint(indexes);
        }

        protected override IEnumerable<Vector2> GetRefreshedTextureCoordinates()
        {
            if (Grid == null)
                return Enumerable.Empty<Vector2>();

            return MeshHelpers.GetOrthographicTextureCoordinates(this);
        }

        protected override IEnumerable<int> GetRefreshedIndices()
        {
            if (Grid == null)
                yield break;

            int[] indexes = Grid.GetResetIndex();
            while (Grid.MoveIndex(indexes))
            {
                if (!MeshingBehavior(indexes[1], indexes[0], Grid[indexes]))
                    continue;

                yield return GetVertexIndex(indexes[0], indexes[1]);
                yield return GetVertexIndex(indexes[0], indexes[1] + 1);
                yield return GetVertexIndex(indexes[0] + 1, indexes[1]);

                yield return GetVertexIndex(indexes[0], indexes[1] + 1);
                yield return GetVertexIndex(indexes[0] + 1, indexes[1] + 1);
                yield return GetVertexIndex(indexes[0] + 1, indexes[1]);
            }

            int GetVertexIndex(int i, int j) => i * (Grid.Dimension.Columns + 1) + j;
        }

        protected override sealed int GetRefreshedVertexCount()
        {
            if (Grid == null)
                return 0;
            return (Grid.Dimension.Rows + 1) * (Grid.Dimension.Columns + 1);
        }

        protected override sealed int GetRefreshedIndexCount()
        {
            if (Grid == null)
                return 0;
            return GetRefreshedIndices().Count();
        }
    }
}