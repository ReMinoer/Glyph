using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class PoorGrid<T> : GridBase<T>
    {
        private readonly Dictionary<Point, T> _items;
        private Func<T> _defaultValueFactory;
        private T _defaultValue;
        public override Func<T> OutOfBoundsValueFactory { get; set; }

        public Func<T> DefaultValueFactory
        {
            get { return _defaultValueFactory; }
            set
            {
                _defaultValueFactory = value;
                _defaultValue = _defaultValueFactory();
            }
        }

        protected override bool HasLowEntropyProtected
        {
            get { return true; }
        }

        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected
        {
            get { return _items.Select<KeyValuePair<Point, T>, IGridCase<T>>(x => new GridCase<T>(x.Key, x.Value)); }
        }

        public IEnumerable<IGridCase<T>> SignificantCases
        {
            get { return SignificantCasesProtected; }
        }

        public PoorGrid(IRectangle rectangle, int columns, int rows)
            : base(rectangle, columns, rows)
        {
            _items = new Dictionary<Point, T>();
        }

        public PoorGrid(int columns, int rows, Vector2 origin, Vector2 delta)
            : base(columns, rows, origin, delta)
        {
            _items = new Dictionary<Point, T>();
        }

        public override T this[int i, int j]
        {
            get
            {
                if (i < 0 || i >= Dimension.Rows || j < 0 || j >= Dimension.Columns)
                    return OutOfBoundsValueFactory();

                T value;
                if (!_items.TryGetValue(new Point(j, i), out value))
                    value = DefaultValueFactory();
                return value;
            }
            set
            {
                if (i < 0 || i >= Dimension.Rows || j < 0 || j >= Dimension.Columns)
                    throw new IndexOutOfRangeException();

                if (value.Equals(_defaultValue))
                {
                    var point = new Point(i, j);
                    if (_items.ContainsKey(point))
                        _items.Remove(point);
                    return;
                }

                _items[new Point(j, i)] = value;
            }
        }

        public void RemoveSignificantCase(Point point)
        {
            _items.Remove(point);
        }

        public void ClearSignificantCases()
        {
            _items.Clear();
        }

        protected override T[][] ToArrayProtected()
        {
            var array = new T[Dimension.Rows][];
            for (int i = 0; i < Dimension.Rows; i++)
            {
                array[i] = new T[Dimension.Columns];
                for (int j = 0; j < Dimension.Columns; j++)
                {
                    T value;
                    if (!_items.TryGetValue(new Point(j, i), out value))
                        value = DefaultValueFactory();

                    array[i][j] = value;
                }
            }

            return array;
        }
    }
}