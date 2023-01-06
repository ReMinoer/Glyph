using System;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Graphics.Meshes.Base;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Simulacra.Utils;

namespace Glyph.Graphics.Meshes
{
    public class GridMesh<TGrid, T, TInfo> : ComplexProceduralMeshBase
        where TGrid : class, IDirtableGrid<T>
        where T : class, IDirtableGridCell<T>
    {
        private Sample[,] _samples;
        private readonly HashSet<Sample> _dirtySamples;
        private bool _dirtySampleArray;

        private TGrid _grid;
        public TGrid Grid
        {
            get => _grid;
            set
            {
                if (_grid == value)
                    return;

                if (_grid != null)
                    _grid.CellsDirtied -= OnGridCellsDirtied;

                _grid = value;
                DirtySampleArray();

                if (_grid != null)
                    _grid.CellsDirtied += OnGridCellsDirtied;

                void OnGridCellsDirtied(object sender, CellsDirtiedEventArgs<T> e)
                {
                    if (e.IsGlobalChange)
                    {
                        DirtySampleArray();
                        return;
                    }

                    foreach (IDirtableGridCell<T> cell in e.Cells)
                        DirtySample(cell.GridPoint);
                }
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
                DirtyAllSamples();

                if (_meshingBehavior != null)
                    _meshingBehavior.Changed += OnMeshingChanged;
        
                void OnMeshingChanged(object sender, EventArgs e) => DirtyAllSamples();
            }
        }
        
        private Point? _samplingSize;
        public Point? SamplingSize
        {
            get => _samplingSize;
            set
            {
                if (_samplingSize == value)
                    return;

                _samplingSize = value;
                DirtySampleArray();
            }
        }

        public override PrimitiveType Type => PrimitiveType.TriangleList;

        public Color Color { get; set; } = Color.White;
        protected override Color GetColor(int vertexIndex) => Color;

        public GridMesh()
        {
            _samples = new Sample[0, 0];
            _dirtySamples = new HashSet<Sample>();
        }

        private void DirtySampleArray()
        {
            _dirtySampleArray = true;
            DirtyCaches();
        }

        private void DirtyAllSamples()
        {
            _dirtySamples.AddMany(_samples.Cast<Sample>());
            DirtyCaches();
        }

        private void DirtySample(Point gridPoint)
        {
            if (SamplingSize.HasValue)
            {
                Point samplingSize = SamplingSize.Value;
                int x = gridPoint.X / samplingSize.X;
                int y = gridPoint.Y / samplingSize.Y;

                // TODO: Dirty neighbor samples if on border
                _dirtySamples.Add(_samples[y, x]);
            }

            DirtyCaches();
        }

        protected override void RefreshCache(IndexedVertexCollection indexedVertices)
        {
            // TODO: Use sorted list
            indexedVertices.Clear();

            if (Grid is null || MeshingBehavior is null)
                return;

            if (SamplingSize is null)
            {
                int gridColumns = Grid.Dimension.Columns;
                int gridRows = Grid.Dimension.Rows;
                
                RefreshSample(indexedVertices, Grid, gridColumns, gridRows);
                return;
            }
            
            ReSampleGridIfNecessary();
            RefreshDirtySamples();
            CopySamplesToCache(indexedVertices);
        }

        private void ReSampleGridIfNecessary()
        {
            if (!_dirtySampleArray)
                return;

            _dirtySamples.Clear();

            int gridColumns = Grid.Dimension.Columns;
            int gridRows = Grid.Dimension.Rows;

            Point gridSize = new Point(Grid.GetLength(1), Grid.GetLength(0));
            Point sampleSize = SamplingSize ?? new Point(gridColumns, gridRows);

            int sampleColumns = (int)MathF.Ceiling((float)gridColumns / sampleSize.X);
            int sampleRows = (int)MathF.Ceiling((float)gridRows / sampleSize.Y);

            var samples = new Sample[sampleRows, sampleColumns];

            samples.GetResetIndex(out int y, out int x);
            while (samples.MoveIndex(ref y, ref x))
            {
                int i = y * sampleSize.Y;
                int j = x * sampleSize.X;
                var sampleRange = new IndexRange(i, j, System.Math.Min(sampleSize.Y, gridSize.Y - i), System.Math.Min(sampleSize.X, gridSize.X - j));
                var sample = new Sample(sampleRange, new List<Vector2>());

                samples[y, x] = sample;
                _dirtySamples.Add(sample);
            }

            _samples = samples;
            _dirtySampleArray = false;
        }

        private void RefreshDirtySamples()
        {
            int gridColumns = Grid.Dimension.Columns;
            int gridRows = Grid.Dimension.Rows;

            foreach (Sample dirtySample in _dirtySamples)
            {
                dirtySample.Vertices.Clear();
                RefreshSample(dirtySample.Vertices, dirtySample.IndexRange, gridColumns, gridRows);
            }

            _dirtySamples.Clear();
        }

        private void RefreshSample(ICollection<Vector2> vertices, IArrayDefinition sampleRange, int gridColumns, int gridRows)
        {
            var infos = new TInfo[gridRows, gridColumns];

            sampleRange.GetResetIndex(out int i, out int j);
            while (sampleRange.MoveIndex(ref i, ref j))
            {
                infos[i, j] = MeshingBehavior.GetCellInfo(Grid, i, j, gridColumns, gridRows);
            }

            sampleRange.GetResetIndex(out i, out j);

            if (MeshingBehavior.CanContainsRectangles)
            {
                while (sampleRange.MoveIndex(ref i, ref j))
                {
                    TInfo cellInfo = infos[i, j];

                    if (MeshingBehavior.IsPartOfRectangle(cellInfo))
                    {
                        MeshingBehavior.AddRectangle(vertices, Grid, GetRectangle(infos, sampleRange, i, j));
                    }
                    else if (MeshingBehavior.IsCell(cellInfo))
                    {
                        MeshingBehavior.AddCell(vertices, Grid, i, j, cellInfo);
                    }
                }
            }
            else
            {
                while (sampleRange.MoveIndex(ref i, ref j))
                {
                    TInfo cellInfo = infos[i, j];

                    if (MeshingBehavior.IsCell(infos[i, j]))
                    {
                        MeshingBehavior.AddCell(vertices, Grid, i, j, cellInfo);
                    }
                }
            }
        }

        private Rectangle GetRectangle(TInfo[,] infos, IArrayDefinition sampleRange, int i, int j)
        {
            // TODO: Stop rectangles on extruded neighbors
            // Get width on first row
            int width = 1;
            int jj = j;
            if (sampleRange.MoveIndex(ref jj, dimension: 1))
            {
                while (MeshingBehavior.IsPartOfRectangle(infos[i, jj]))
                {
                    infos[i, jj] = MeshingBehavior.EmptyInfo;
                    width++;
                    if (!sampleRange.MoveIndex(ref jj, dimension: 1))
                        break;
                }
            }

            // Add to height all rows matching width
            int height = 1;
            int ii = i;
            while (sampleRange.MoveIndex(ref ii, dimension: 0))
            {
                bool matchingFirstRow = true;
                for (jj = j; jj < j + width; jj++)
                {
                    if (!MeshingBehavior.IsPartOfRectangle(infos[ii, jj]))
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
                    infos[ii, jj] = MeshingBehavior.EmptyInfo;
                }
            }

            return new Rectangle(j, i, width, height);
        }

        private void CopySamplesToCache(IndexedVertexCollection indexedVertices)
        {
            _samples.GetResetIndex(out int i, out int j);
            while (_samples.MoveIndex(ref i, ref j))
            {
                List<Vector2> sampleVertices = _samples[i, j].Vertices;

                for (int v = 0; v < sampleVertices.Count; v++)
                    indexedVertices.Add(sampleVertices[v]);
            }
        }

        private class Sample
        {
            public IndexRange IndexRange { get; }
            public List<Vector2> Vertices { get; }

            public Sample(IndexRange indexRange, List<Vector2> vertices)
            {
                IndexRange = indexRange;
                Vertices = vertices;
            }
        }
    }
}