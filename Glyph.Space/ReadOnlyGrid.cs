using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public class ReadOnlyGrid<T> : IGrid<T>
    {
        private readonly IGrid<T> _grid;

        public bool IsVoid => _grid.IsVoid;
        public TopLeftRectangle BoundingBox => _grid.BoundingBox;
        public Vector2 Center => _grid.Center;

        public Vector2 Delta => _grid.Delta;
        public GridDimension Dimension => _grid.Dimension;

        public ITransformation Transformation
        {
            get => _grid.Transformation;
            set => _grid.Transformation = value;
        }

        public bool HasLowEntropy => _grid.HasLowEntropy;
        public IEnumerable<T> Values => _grid.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => _grid.SignificantCases;

        public T this[int i, int j] => _grid[i, j];
        public T this[Point gridPoint] => _grid[gridPoint];
        public T this[Vector2 worldPoint] => _grid[worldPoint];
        object ITwoDimensionArray.this[int i, int j] => _grid[i, j];

        public event ArrayChangedEventHandler ArrayChanged;

        public ReadOnlyGrid(IGrid<T> grid)
        {
            _grid = grid;
            _grid.ArrayChanged += OnArrayChanged;
        }

        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            ArrayChanged?.Invoke(this, e);
        }

        public bool ContainsPoint(Vector2 point) => _grid.ContainsPoint(point);
        public bool Intersects(Segment segment) => _grid.Intersects(segment);
        public bool Intersects<TShape>(TShape edgedShape) where TShape : IEdgedShape => _grid.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _grid.Intersects(circle);
        public Vector2 ToWorldPoint(Point gridPoint) => _grid.ToWorldPoint(gridPoint);
        public Point ToGridPoint(Vector2 worldPoint) => _grid.ToGridPoint(worldPoint);
        public IEnumerator<T> GetEnumerator() => _grid.GetEnumerator();

        int IArrayDefinition.Rank => _grid.Rank;
        int IArrayDefinition.GetLowerBound(int dimension) => _grid.GetLowerBound(dimension);
        int IArrayDefinition.GetLength(int dimension) => _grid.GetLength(dimension);
        object IArray.this[params int[] indexes] => this[indexes[0], indexes[1]];
        T IArray<T>.this[params int[] indexes] => this[indexes[0], indexes[1]];
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_grid).GetEnumerator();
    }
}