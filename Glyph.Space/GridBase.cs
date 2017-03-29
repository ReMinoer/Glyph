using System;
using System.Collections;
using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Diese.Collections;

namespace Glyph.Space
{
    // TODO : Clamp methods to grid dimensions
    public class GridBase : IGrid
    {
        public TopLeftRectangle Rectangle { get; private set; }
        public GridDimension Dimension { get; private set; }
        public Vector2 Delta { get; private set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(0, 0, Dimension.Columns, Dimension.Rows); }
        }

        public Vector2 Center
        {
            get { return Rectangle.Center; }
            set { Rectangle = new TopLeftRectangle {Center = value, Size = Rectangle.Size}; }
        }

        public GridBase(TopLeftRectangle rectangle, int columns, int rows)
        {
            Rectangle = rectangle;
            Dimension = new GridDimension(columns, rows);

            Delta = Rectangle.Size / Dimension;
        }

        public GridBase(int columns, int rows, Vector2 origin, Vector2 delta)
        {
            Dimension = new GridDimension(columns, rows);
            Delta = delta;

            Rectangle = new TopLeftRectangle(origin, delta * new Vector2(columns, rows));
        }

        public TopLeftRectangle BoundingBox
        {
            get { return Rectangle; }
        }

        public bool ContainsPoint(Vector2 worldPoint)
        {
            return Rectangle.ContainsPoint(worldPoint);
        }

        public bool ContainsPoint(int i, int j)
        {
            return j >= 0 && j < Dimension.Columns && i >= 0 && i < Dimension.Rows;
        }

        public bool ContainsPoint(Point gridPoint)
        {
            return ContainsPoint(gridPoint.Y, gridPoint.X);
        }

        public Vector2 ToWorldPoint(int i, int j)
        {
            return ToWorldPoint(new Point(j, i));
        }

        public Vector2 ToWorldPoint(Point gridPoint)
        {
            return Rectangle.Position + Delta.Integrate(gridPoint);
        }

        public Vector2 ToWorldPoint(IGridPositionable gridPositionable)
        {
            return ToWorldPoint(gridPositionable.GridPosition);
        }

        public TopLeftRectangle ToWorldRange(int x, int y, int width, int height)
        {
            return ToWorldRange(new Rectangle(x, y, width, height));
        }

        public TopLeftRectangle ToWorldRange(Rectangle rectangle)
        {
            return new TopLeftRectangle(ToWorldPoint(rectangle.Location), Delta.Integrate(rectangle.Size) + Delta);
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            return Delta.Discretize(worldPoint - Rectangle.Position);
        }

        public Rectangle ToGridRange(TopLeftRectangle rectangle)
        {
            Point position = ToGridPoint(rectangle.Position);
            return new Rectangle(position, ToGridPoint(rectangle.P2) - position + new Point(1, 1));
        }

        public bool Intersects(TopLeftRectangle rectangle)
        {
            return Rectangle.Intersects(rectangle);
        }

        public bool Intersects(Circle circle)
        {
            return Rectangle.Intersects(circle);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return new PointEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class PointEnumerator : IEnumerator<Point>
        {
            private readonly IGrid _grid;
            private int _position = -1;

            public PointEnumerator(IGrid grid)
            {
                _grid = grid;
            }

            public bool MoveNext()
            {
                _position++;
                return (_position < _grid.Dimension.Rows * _grid.Dimension.Columns);
            }

            public void Reset()
            {
                _position = -1;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public Point Current
            {
                get { return new Point(_position % _grid.Dimension.Columns, _position / _grid.Dimension.Columns); }
            }

            public void Dispose()
            {
            }
        }
    }

    public abstract class GridBase<T> : GridBase, IWriteableGrid<T>
    {
        IEnumerable<IGridCase<T>> IGrid<T>.SignificantCases
        {
            get { return SignificantCasesProtected; }
        }

        protected abstract IEnumerable<IGridCase<T>> SignificantCasesProtected { get; }

        bool IGrid<T>.HasLowEntropy
        {
            get { return HasLowEntropyProtected; }
        }

        protected abstract bool HasLowEntropyProtected { get; }

        public IEnumerable<T> Values
        {
            get { return new Enumerable<T>(new ValueEnumerator(this)); }
        }

        protected GridBase(TopLeftRectangle rectangle, int columns, int rows)
            : base(rectangle, columns, rows)
        {
        }

        protected GridBase(int columns, int rows, Vector2 origin, Vector2 delta)
            : base(columns, rows, origin, delta)
        {
        }

        public abstract T this[int i, int j] { get; set; }

        public T this[Point gridPoint]
        {
            get { return this[gridPoint.Y, gridPoint.X]; }
            set { this[gridPoint.Y, gridPoint.X] = value; }
        }

        public T this[Vector2 worldPoint]
        {
            get { return this[ToGridPoint(worldPoint)]; }
            set { this[ToGridPoint(worldPoint)] = value; }
        }

        public T this[IGridPositionable gridPositionable]
        {
            get { return this[gridPositionable.GridPosition]; }
            set { this[gridPositionable.GridPosition] = value; }
        }

        public void Fill(Func<T> valueFactory)
        {
            Fill(valueFactory, Point.Zero, new Point(Dimension.Columns, Dimension.Rows));
        }

        public void Fill(Func<T> valueFactory, Point minPoint, Point maxPoint)
        {
            for (int i = minPoint.Y; i < maxPoint.Y; i++)
                for (int j = minPoint.X; j < maxPoint.X; j++)
                    this[i, j] = valueFactory();
        }

        T[][] IGrid<T>.ToArray()
        {
            return ToArrayProtected();
        }

        protected abstract T[][] ToArrayProtected();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class ValueEnumerator : IEnumerator<T>
        {
            private readonly IGrid<T> _grid;
            private int _position = -1;

            public ValueEnumerator(IGrid<T> grid)
            {
                _grid = grid;
            }

            public bool MoveNext()
            {
                _position++;
                return (_position < _grid.Dimension.Rows * _grid.Dimension.Columns);
            }

            public void Reset()
            {
                _position = -1;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public T Current
            {
                get { return _grid[_position / _grid.Dimension.Columns, _position % _grid.Dimension.Columns]; }
            }

            public void Dispose()
            {
            }
        }
    }
}