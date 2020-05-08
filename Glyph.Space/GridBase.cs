using System;
using System.Collections;
using System.Collections.Generic;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Diese.Collections;
using Simulacra.Utils;

namespace Glyph.Space
{
    public abstract class GridBase<T> : Grid, IWriteableGrid<T>
    {
        public IEnumerable<T> Values => new Enumerable<T>(new ValueEnumerator(this));

        protected abstract IEnumerable<IGridCase<T>> SignificantCasesProtected { get; }
        IEnumerable<IGridCase<T>> IGrid<T>.SignificantCases => SignificantCasesProtected;

        protected abstract bool HasLowEntropyProtected { get; }
        bool IGrid<T>.HasLowEntropy => HasLowEntropyProtected;

        public event ArrayChangedEventHandler ArrayChanged;

        protected GridBase(TopLeftRectangle rectangle, int columns, int rows)
            : base(rectangle, columns, rows)
        {
        }

        protected GridBase(int columns, int rows, Vector2 origin, Vector2 delta)
            : base(columns, rows, origin, delta)
        {
        }

        protected abstract T GetValue(int i, int j);
        protected abstract void SetValue(int i, int j, T value);

        public T this[int i, int j]
        {
            get => GetValue(i, j);
            set
            {
                if (ArrayChanged == null)
                {
                    SetValue(i, j, value);
                }
                else
                {
                    if (EqualityComparer<T>.Default.Equals(GetValue(i, j), value))
                        return;

                    SetValue(i, j, value);
                    ArrayChanged?.Invoke(this, new ArrayChangedEventArgs
                    {
                        StartingIndexes = new []{i, j},
                        NewValues = new []{value}
                    });
                }
            }
        }

        T IArray<T>.this[params int[] indexes] => this[indexes[0], indexes[1]];
        T IWriteableArray<T>.this[params int[] indexes]
        {
            get => this[indexes[0], indexes[1]];
            set => this[indexes[0], indexes[1]] = value;
        }

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
            if (ArrayChanged == null)
            {
                for (int i = minPoint.Y; i < maxPoint.Y; i++)
                    for (int j = minPoint.X; j < maxPoint.X; j++)
                        SetValue(i, j, valueFactory());
            }
            else
            {
                Point counts = maxPoint - minPoint;
                var newValues = new T[counts.Y, counts.X];

                for (int i = 0; i < counts.Y; i++)
                    for (int j = 0; j < counts.X; j++)
                    {
                        T value = valueFactory();

                        SetValue(i + minPoint.Y, j + minPoint.X, value);
                        newValues[i, j] = value;
                    }

                ArrayChanged?.Invoke(this, new ArrayChangedEventArgs
                {
                    StartingIndexes = new []{minPoint.Y, minPoint.X},
                    NewValues = newValues
                });
            }
        }

        T[][] IGrid<T>.ToArray() => ToArrayProtected();
        protected abstract T[][] ToArrayProtected();

        public IEnumerator<T> GetEnumerator() => new ValueEnumerator(this);
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