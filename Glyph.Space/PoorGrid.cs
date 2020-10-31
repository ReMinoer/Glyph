using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public class PoorGrid<T> : GridBase<T>
    {
        private readonly Dictionary<Point, T> _items;
        private Func<T> _defaultValueFactory;
        private T _defaultValue;

        public Func<T> DefaultValueFactory
        {
            get => _defaultValueFactory;
            set
            {
                _defaultValueFactory = value;
                _defaultValue = _defaultValueFactory();
            }
        }

        public override GridDimension Dimension
        {
            get => base.Dimension;
            set => Resize(value.ToArray(), keepValues: true);
        }

        protected override bool HasLowEntropyProtected => true;
        public IEnumerable<IGridCase<T>> SignificantCases => SignificantCasesProtected;
        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected
        {
            get { return _items.Select<KeyValuePair<Point, T>, IGridCase<T>>(x => new GridCase<T>(x.Key, x.Value)); }
        }

        public PoorGrid(int columns, int rows, Vector2 origin, Vector2 delta)
            : base(columns, rows, origin, delta)
        {
            _items = new Dictionary<Point, T>();
        }

        public override void Resize(int[] newLengths, bool keepValues = true, Func<T, int[], T> valueFactory = null)
        {
            base.Dimension = new GridDimension(newLengths[1], newLengths[0]);

            foreach (Point point in _items.Keys)
            {
                if (point.Y < Dimension.Rows && point.X < Dimension.Columns)
                    continue;

                RemoveSignificantCase(point);
            }

            if (valueFactory != null)
                DefaultValueFactory = () => valueFactory(default, Array.Empty<int>());
        }

        protected override T GetValue(int i, int j)
        {
            if (i < 0 || i >= Dimension.Rows || j < 0 || j >= Dimension.Columns)
                throw new IndexOutOfRangeException();

            if (!_items.TryGetValue(new Point(j, i), out T value))
                value = DefaultValueFactory();

            return value;
        }

        protected override void SetValue(int i, int j, T value)
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

        public void RemoveSignificantCase(Point point)
        {
            _items.Remove(point);
        }

        public void ClearSignificantCases()
        {
            _items.Clear();
        }
    }
}