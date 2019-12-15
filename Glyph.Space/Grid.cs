using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Diese.Collections;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    // TODO : Clamp methods to grid dimensions
    public class Grid : IGrid
    {
        public TopLeftRectangle Rectangle { get; private set; }
        public GridDimension Dimension { get; private set; }
        public Vector2 Delta { get; private set; }
        public bool IsVoid => (Dimension.Columns == 0 && Dimension.Rows == 0) || Delta == Vector2.Zero;
        public TopLeftRectangle BoundingBox => Rectangle;
        public Rectangle Bounds => new Rectangle(0, 0, Dimension.Columns, Dimension.Rows);

        public Vector2 Center
        {
            get => Rectangle.Center;
            set => Rectangle = new TopLeftRectangle { Center = value, Size = Rectangle.Size };
        }

        int IArray.Rank => 2;
        int IArray.GetLength(int dimension)
        {
            switch (dimension)
            {
                case 0: return Dimension.Rows;
                case 1: return Dimension.Columns;
                default: throw new NotSupportedException();
            }
        }

        public Grid(TopLeftRectangle rectangle, int columns, int rows)
        {
            Rectangle = rectangle;
            Dimension = new GridDimension(columns, rows);

            Delta = Rectangle.Size / Dimension;
        }

        public Grid(int columns, int rows, Vector2 origin, Vector2 delta)
        {
            Dimension = new GridDimension(columns, rows);
            Delta = delta;

            Rectangle = new TopLeftRectangle(origin, delta * new Vector2(columns, rows));
        }

        public bool ContainsPoint(Vector2 worldPoint) => Rectangle.ContainsPoint(worldPoint);
        public bool Intersects(Segment segment) => Rectangle.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => Rectangle.Intersects(edgedShape);
        public bool Intersects(Circle circle) => Rectangle.Intersects(circle);

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
            return new Rectangle(position, ToGridPoint(rectangle.P3) - position + new Point(1, 1));
        }
    }

    public class Grid<T> : GridBase<T>
    {
        private readonly T[][] _data;

        protected override bool HasLowEntropyProtected => false;
        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected => new Enumerable<IGridCase<T>>(new Enumerator(this));

        public Grid(TopLeftRectangle rectangle, int columns, int rows)
            : base(rectangle, columns, rows)
        {
            _data = new T[Dimension.Rows][];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = new T[Dimension.Columns];
        }

        public Grid(int columns, int rows, Vector2 origin, Vector2 delta)
            : base(columns, rows, origin, delta)
        {
            _data = new T[Dimension.Rows][];
            for (int i = 0; i < _data.Length; i++)
                _data[i] = new T[Dimension.Columns];
        }

        protected override T GetValue(int i, int j) => _data[i][j];
        protected override void SetValue(int i, int j, T value) => _data[i][j] = value;

        protected override T[][] ToArrayProtected() => _data.ToArray();

        public class Enumerator : IEnumerator<IGridCase<T>>
        {
            private readonly Grid<T> _grid;
            private int _position = -1;

            public IGridCase<T> Current
            {
                get
                {
                    var point = new Point(_position % _grid.Dimension.Columns, _position / _grid.Dimension.Columns);
                    return new GridCase<T>(point, _grid[point]);
                }
            }
            object IEnumerator.Current => Current;

            public Enumerator(Grid<T> grid)
            {
                _grid = grid;
            }

            public bool MoveNext()
            {
                _position++;
                return _position < _grid.Dimension.Rows * _grid.Dimension.Columns;
            }

            public void Reset()
            {
                _position = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}