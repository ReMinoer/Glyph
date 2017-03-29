using System;
using System.Collections;
using System.Collections.Generic;
using Diese.Collections;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class Grid<T> : GridBase<T>
    {
        private readonly T[][] _data;
        public override Func<T> OutOfBoundsValueFactory { get; set; }

        protected override bool HasLowEntropyProtected
        {
            get { return false; }
        }

        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected
        {
            get { return new Enumerable<IGridCase<T>>(new Enumerator(this)); }
        }

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

        public override T this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= Dimension.Rows || j < 0 || j >= Dimension.Columns)
                    return OutOfBoundsValueFactory();
                return _data[i][j];
            }
            set {  _data[i][j] = value; }
        }

        protected override T[][] ToArrayProtected()
        {
            return _data;
        }

        public T[][] GetArray()
        {
            return ToArrayProtected();
        }

        public class Enumerator : IEnumerator<IGridCase<T>>
        {
            private readonly Grid<T> _grid;
            private int _position = -1;

            public Enumerator(Grid<T> grid)
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

            public IGridCase<T> Current
            {
                get
                {
                    var point = new Point(_position % _grid.Dimension.Columns, _position / _grid.Dimension.Columns);
                    return new GridCase<T>(point, _grid[point]);
                }
            }

            public void Dispose()
            {
            }
        }
    }
}