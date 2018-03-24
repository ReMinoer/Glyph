using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class ReadOnlyGrid<T> : IGrid<T>
    {
        private readonly IGrid<T> _grid;

        public bool IsVoid => _grid.IsVoid;
        public TopLeftRectangle BoundingBox => _grid.BoundingBox;
        public Vector2 Center => _grid.Center;
        public Rectangle Bounds => _grid.Bounds;
        public Vector2 Delta => _grid.Delta;
        public GridDimension Dimension => _grid.Dimension;
        public bool HasLowEntropy => _grid.HasLowEntropy;
        public IEnumerable<T> Values => _grid.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => _grid.SignificantCases;

        T IGrid<T>.this[int i, int j] => _grid[i, j];
        T IGrid<T>.this[Point gridPoint] => _grid[gridPoint];
        T IGrid<T>.this[Vector2 worldPoint] => _grid[worldPoint];
        T IGrid<T>.this[IGridPositionable gridPositionable] => _grid[gridPositionable];

        public ReadOnlyGrid(IGrid<T> grid)
        {
            _grid = grid;
        }

        public bool ContainsPoint(Vector2 point) => _grid.ContainsPoint(point);
        public bool Intersects(Segment segment) => _grid.Intersects(segment);
        public bool Intersects<TShape>(TShape edgedShape) where TShape : IEdgedShape => _grid.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _grid.Intersects(circle);
        public Vector2 ToWorldPoint(int i, int j) => _grid.ToWorldPoint(i, j);
        public Vector2 ToWorldPoint(Point gridPoint) => _grid.ToWorldPoint(gridPoint);
        public Vector2 ToWorldPoint(IGridPositionable gridPoint) => _grid.ToWorldPoint(gridPoint);
        public TopLeftRectangle ToWorldRange(int x, int y, int width, int height) => _grid.ToWorldRange(x, y, width, height);
        public TopLeftRectangle ToWorldRange(Rectangle rectangle) => _grid.ToWorldRange(rectangle);
        public Point ToGridPoint(Vector2 worldPoint) => _grid.ToGridPoint(worldPoint);
        public Rectangle ToGridRange(TopLeftRectangle rectangle) => _grid.ToGridRange(rectangle);
        public bool ContainsPoint(int i, int j) => _grid.ContainsPoint(i, j);
        public bool ContainsPoint(Point gridPoint) => _grid.ContainsPoint(gridPoint);
        public T[][] ToArray() => _grid.ToArray();
        public IEnumerator<Point> GetEnumerator() => _grid.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_grid).GetEnumerator();
    }
}