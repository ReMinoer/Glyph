using System;
using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.Utils;

namespace Glyph.Graphics.Meshes
{
    public class GridMesh<TGrid, T, TInfo> : ProceduralMeshBase
        where TGrid : class, IDirtableGrid<T>
        where T : class, IDirtable
    {
        private TGrid _grid;
        public TGrid Grid
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

        private IGridMeshing<TGrid, T, TInfo> _meshingBehavior;
        public IGridMeshing<TGrid, T, TInfo> MeshingBehavior
        {
            get => _meshingBehavior;
            set
            {
                if (_meshingBehavior == value)
                    return;

                if (_meshingBehavior != null)
                    _meshingBehavior.Changed -= OnMeshingChanged;

                _meshingBehavior = value;
                DirtyCaches();

                if (_meshingBehavior != null)
                    _meshingBehavior.Changed += OnMeshingChanged;

                void OnMeshingChanged(object sender, EventArgs e) => DirtyCaches();
            }
        }

        public override PrimitiveType Type => PrimitiveType.TriangleList;

        public Color Color { get; set; } = Color.White;
        protected override Color GetColor(int vertexIndex) => Color;

        protected override void RefreshCache(List<Vector2> vertices, List<int> indices)
        {
            // TODO: Use sorted list
            vertices.Clear();
            indices.Clear();

            if (Grid == null || MeshingBehavior == null)
                return;

            // Create byte mask
            var mask = new TInfo[Grid.GetLength(0), Grid.GetLength(1)];
            mask.GetResetIndex(out int i, out int j);
            while (mask.MoveIndex(ref i, ref j))
            {
                mask[i, j] = MeshingBehavior.GetCellInfo(Grid, i, j);
            }

            // Process mask
            mask.GetResetIndex(out i, out j);
            if (MeshingBehavior.CanContainsRectangles)
            {
                while (mask.MoveIndex(ref i, ref j))
                {
                    TInfo cellInfo = mask[i, j];

                    if (MeshingBehavior.IsPartOfRectangle(cellInfo))
                    {
                        MeshingBehavior.AddRectangle(vertices, indices, Grid, GetRectangle(mask, i, j));
                    }
                    else if (MeshingBehavior.IsCell(cellInfo))
                    {
                        MeshingBehavior.AddCell(vertices, indices, Grid, i, j, cellInfo);
                    }
                }
            }
            else
            {
                while (mask.MoveIndex(ref i, ref j))
                {
                    TInfo cellInfo = mask[i, j];

                    if (MeshingBehavior.IsCell(mask[i, j]))
                    {
                        MeshingBehavior.AddCell(vertices, indices, Grid, i, j, cellInfo);
                    }
                }
            }
        }

        private Rectangle GetRectangle(TInfo[,] mask, int i, int j)
        {
            // Get width on first row
            int width = 1;
            int jj = j;
            if (mask.MoveIndex(ref jj, dimension: 1))
            {
                while (MeshingBehavior.IsPartOfRectangle(mask[i, jj]))
                {
                    mask[i, jj] = MeshingBehavior.EmptyInfo;
                    width++;
                    if (!mask.MoveIndex(ref jj, dimension: 1))
                        break;
                }
            }

            // Add to height all rows matching width
            int height = 1;
            int ii = i;
            while (mask.MoveIndex(ref ii, dimension: 0))
            {
                bool matchingFirstRow = true;
                for (jj = j; jj < j + width; jj++)
                {
                    if (!MeshingBehavior.IsPartOfRectangle(mask[ii, jj]))
                    {
                        matchingFirstRow = false;
                        break;
                    }
                }

                // If current row cannot contains chosen width, fix mask and stop here
                if (!matchingFirstRow)
                    break;

                // Else, increment height and continue with next row
                height++;

                for (jj = j; jj < j + width; jj++)
                {
                    mask[ii, jj] = MeshingBehavior.EmptyInfo;
                }
            }

            return new Rectangle(j, i, width, height);
        }
    }
}