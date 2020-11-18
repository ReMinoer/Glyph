using System;
using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.Meshes.Utils;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes
{
    public class GridMesh<T> : ProceduralMeshBase
        where T : class, IDirtable
    {
        private IDirtableGrid<T> _grid;
        public IDirtableGrid<T> Grid
        {
            get => _grid;
            set
            {
                if (_grid == value)
                    return;

                if (_grid != null)
                    _grid.Dirtied -= OnGridDirtied;

                _grid = value;
                DirtyCaches();

                if (_grid != null)
                    _grid.Dirtied += OnGridDirtied;

                void OnGridDirtied(object sender, EventArgs e) => DirtyCaches();
            }
        }

        private Func<int, int, T, bool> _meshingBehavior;
        public Func<int, int, T, bool> MeshingBehavior
        {
            get => _meshingBehavior;
            set
            {
                if (_meshingBehavior == value)
                    return;

                _meshingBehavior = value;
                DirtyCaches();
            }
        }

        public override PrimitiveType Type => PrimitiveType.TriangleList;
        protected override Color GetColor(int vertexIndex) => Color.White;

        protected override void RefreshCache(List<Vector2> vertices, List<int> indices)
        {
            vertices.Clear();
            indices.Clear();

            if (Grid != null)
            {
                List<Rectangle> rectangles = MeshHelpers.GetRectanglesFromArray(Grid, MeshingBehavior);
                foreach (Rectangle rectangle in rectangles)
                {
                    AddVertex(vertices, indices, rectangle.Top, rectangle.Left);
                    AddVertex(vertices, indices, rectangle.Top, rectangle.Right);
                    AddVertex(vertices, indices, rectangle.Bottom, rectangle.Left);

                    AddVertex(vertices, indices, rectangle.Top, rectangle.Right);
                    AddVertex(vertices, indices, rectangle.Bottom, rectangle.Right);
                    AddVertex(vertices, indices, rectangle.Bottom, rectangle.Left);
                }
            }
        }

        private void AddVertex(List<Vector2> vertices, List<int> indices, int i, int j)
        {
            Vector2 vertex = Grid.ToWorldPoint(i, j);
            int index = vertices.IndexOf(vertex);
            if (index == -1)
            {
                index = vertices.Count;
                vertices.Add(vertex);
            }

            indices.Add(index);
        }
    }
}