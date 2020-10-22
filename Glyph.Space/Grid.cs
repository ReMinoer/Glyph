using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public class Grid : IGrid
    {
        public TopLeftRectangle Rectangle { get; private set; }
        public GridDimension Dimension { get; }
        public Vector2 Delta { get; }
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

        object IArray.this[params int[] indexes] => null;

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

        public Vector2 ToWorldPoint(Point gridPoint)
        {
            return Rectangle.Position + Delta.Integrate(gridPoint);
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            return Delta.Discretize(worldPoint - Rectangle.Position);
        }
    }

    public class Grid<T> : GridBase<T>
    {
        private readonly ITwoDimensionWriteableArray<T> _data;

        protected override bool HasLowEntropyProtected => false;
        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected => new Enumerable<IGridCase<T>>(new Enumerator(this));

        public Grid(TopLeftRectangle rectangle, int columns, int rows, Func<int[], T> defaultCellValueFactory = null)
            : base(rectangle, columns, rows)
        {
            _data = new TwoDimensionArray<T>(new T[Dimension.Rows, Dimension.Columns]);

            if (defaultCellValueFactory != null)
                _data.Fill(defaultCellValueFactory);
        }

        public Grid(int columns, int rows, Vector2 origin, Vector2 delta, Func<int[], T> defaultCellValueFactory = null)
            : base(columns, rows, origin, delta)
        {
            _data = new TwoDimensionArray<T>(new T[Dimension.Rows, Dimension.Columns]);

            if (defaultCellValueFactory != null)
                _data.Fill(defaultCellValueFactory);
        }

        public Grid(TopLeftRectangle rectangle, ITwoDimensionWriteableArray<T> data)
            : base(rectangle, data.GetLength(1), data.GetLength(0))
        {
            _data = data;
        }

        public Grid(ITwoDimensionWriteableArray<T> data, Vector2 origin, Vector2 delta)
            : base(data.GetLength(1), data.GetLength(0), origin, delta)
        {
            _data = data;
        }

        protected override T GetValue(int i, int j) => _data[i, j];
        protected override void SetValue(int i, int j, T value) => _data[i, j] = value;

        protected override T[][] ToArrayProtected()
        {
            var array = new T[_data.GetLength(0)][];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                array[i] = new T[_data.GetLength(1)];
                for (int j = 0; j < array.GetLength(1); j++)
                    array[i][j] = _data[i, j];
            }
            return array;
        }

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