using System.Collections;
using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class ReadOnlyGrid<T> : IGrid<T>
    {
        private readonly IGrid<T> _gridImplementation;

        public bool IsVoid => _gridImplementation.IsVoid;

        public TopLeftRectangle BoundingBox
        {
            get { return _gridImplementation.BoundingBox; }
        }

        public Vector2 Center
        {
            get { return _gridImplementation.Center; }
        }

        public Rectangle Bounds
        {
            get { return _gridImplementation.Bounds; }
        }

        public Vector2 Delta
        {
            get { return _gridImplementation.Delta; }
        }

        public GridDimension Dimension
        {
            get { return _gridImplementation.Dimension; }
        }

        public bool HasLowEntropy
        {
            get { return _gridImplementation.HasLowEntropy; }
        }

        public IEnumerable<T> Values
        {
            get { return _gridImplementation.Values; }
        }

        public IEnumerable<IGridCase<T>> SignificantCases
        {
            get { return _gridImplementation.SignificantCases; }
        }

        T IGrid<T>.this[int i, int j]
        {
            get { return _gridImplementation[i, j]; }
        }

        T IGrid<T>.this[Point gridPoint]
        {
            get { return _gridImplementation[gridPoint]; }
        }

        T IGrid<T>.this[Vector2 worldPoint]
        {
            get { return _gridImplementation[worldPoint]; }
        }

        T IGrid<T>.this[IGridPositionable gridPositionable]
        {
            get { return _gridImplementation[gridPositionable]; }
        }

        public T[][] ToArray()
        {
            return _gridImplementation.ToArray();
        }

        public ReadOnlyGrid(IGrid<T> gridImplementation)
        {
            _gridImplementation = gridImplementation;
        }

        public bool ContainsPoint(Vector2 point)
        {
            return _gridImplementation.ContainsPoint(point);
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return _gridImplementation.Intersects(rectangle);
        }

        public bool Intersects(Circle circle)
        {
            return _gridImplementation.Intersects(circle);
        }

        public Vector2 ToWorldPoint(int i, int j)
        {
            return _gridImplementation.ToWorldPoint(i, j);
        }

        public Vector2 ToWorldPoint(Point gridPoint)
        {
            return _gridImplementation.ToWorldPoint(gridPoint);
        }

        public Vector2 ToWorldPoint(IGridPositionable gridPoint)
        {
            return _gridImplementation.ToWorldPoint(gridPoint);
        }

        public TopLeftRectangle ToWorldRange(int x, int y, int width, int height)
        {
            return _gridImplementation.ToWorldRange(x, y, width, height);
        }

        public TopLeftRectangle ToWorldRange(Rectangle rectangle)
        {
            return _gridImplementation.ToWorldRange(rectangle);
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            return _gridImplementation.ToGridPoint(worldPoint);
        }

        public Rectangle ToGridRange(TopLeftRectangle rectangle)
        {
            return _gridImplementation.ToGridRange(rectangle);
        }

        public bool ContainsPoint(int i, int j)
        {
            return _gridImplementation.ContainsPoint(i, j);
        }

        public bool ContainsPoint(Point gridPoint)
        {
            return _gridImplementation.ContainsPoint(gridPoint);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return _gridImplementation.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_gridImplementation).GetEnumerator();
        }
    }
}