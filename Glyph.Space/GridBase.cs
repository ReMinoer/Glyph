using System;
using System.Collections;
using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Diese.Collections;

namespace Glyph.Space
{
    public abstract class GridBase<T> : Grid, IWriteableGrid<T>
    {
        protected abstract IEnumerable<IGridCase<T>> SignificantCasesProtected { get; }
        protected abstract bool HasLowEntropyProtected { get; }

        IEnumerable<IGridCase<T>> IGrid<T>.SignificantCases => SignificantCasesProtected;
        bool IGrid<T>.HasLowEntropy => HasLowEntropyProtected;

        public IEnumerable<T> Values => new Enumerable<T>(new ValueEnumerator(this));

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
            get => this[gridPoint.Y, gridPoint.X];
            set => this[gridPoint.Y, gridPoint.X] = value;
        }

        public T this[Vector2 worldPoint]
        {
            get => this[ToGridPoint(worldPoint)];
            set => this[ToGridPoint(worldPoint)] = value;
        }

        public T this[IGridPositionable gridPositionable]
        {
            get => this[gridPositionable.GridPosition];
            set => this[gridPositionable.GridPosition] = value;
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

        T[][] IGrid<T>.ToArray() => ToArrayProtected();
        protected abstract T[][] ToArrayProtected();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class ValueEnumerator : IEnumerator<T>
        {
            private readonly IGrid<T> _grid;
            private int _position = -1;

            public T Current => _grid[_position / _grid.Dimension.Columns, _position % _grid.Dimension.Columns];
            object IEnumerator.Current => Current;

            public ValueEnumerator(IGrid<T> grid)
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