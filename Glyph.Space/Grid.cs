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
    public class Grid : IResizeableGrid
    {
        public ITransformation Transformation { get; set; }

        private GridDimension _dimension;
        public virtual GridDimension Dimension
        {
            get => _dimension;
            set
            {
                if (_dimension.Equals(value))
                    return;

                if (IsNotifying)
                {
                    GridDimension previousValue = _dimension;

                    _dimension = value;

                    NotifyArrayChanged(ArrayChangedEventArgs.Resize(value.ToArray(), previousValue.ToArray()));
                }
                else
                {
                    _dimension = value;
                }
            }
        }

        public Vector2 Delta { get; set; }
        public Quad Shape => Transformation.Transform(new TopLeftRectangle(Vector2.Zero, Delta * Dimension));

        public bool IsVoid => (Dimension.Columns == 0 && Dimension.Rows == 0) || Delta == Vector2.Zero;
        public TopLeftRectangle BoundingBox => Shape.BoundingBox;
        public Vector2 Center => Shape.Center;

        int IArrayDefinition.Rank => 2;
        int IArrayDefinition.GetLength(int dimension)
        {
            switch (dimension)
            {
                case 0: return Dimension.Rows;
                case 1: return Dimension.Columns;
                default: throw new NotSupportedException();
            }
        }

        int[] IResizeableArrayDefinition.Lengths
        {
            set => Dimension = new GridDimension(value[1], value[0]);
        }

        public event ArrayChangedEventHandler ArrayChanged;

        public Grid(int columns, int rows, Vector2 delta)
        {
            _dimension = new GridDimension(columns, rows);
            Delta = delta;
        }

        public bool ContainsPoint(Vector2 worldPoint) => Shape.ContainsPoint(worldPoint);
        public bool Intersects(Segment segment) => Shape.Intersects(segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => Shape.Intersects(edgedShape);
        public bool Intersects(Circle circle) => Shape.Intersects(circle);

        public Vector2 ToWorldPoint(Point gridPoint)
        {
            if (Transformation != null)
                return Transformation.Transform(Delta.Integrate(gridPoint));

            return Delta.Integrate(gridPoint);
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            if (Transformation != null)
                return Delta.Discretize(Transformation.InverseTransform(worldPoint));

            return Delta.Discretize(worldPoint); 
        }

        protected bool IsNotifying => ArrayChanged != null;
        protected void NotifyArrayChanged(ArrayChangedEventArgs e) => ArrayChanged?.Invoke(this, e);

        public int[] GetResetIndex() => ArrayUtils.GetResetIndex(this);
        public bool MoveIndex(int[] indexes) => ArrayUtils.MoveIndex(this, indexes);
    }

    public class Grid<T> : GridBase<T>
    {
        private readonly TwoDimensionArray<T> _data;
        private readonly Func<T, int[], T> _defaultCellValueFactory;

        public override sealed GridDimension Dimension
        {
            get => new GridDimension(_data.Lengths[1], _data.Lengths[0]);
            set
            {
                _data.Resize(value.ToArray(), keepValues: true, _defaultCellValueFactory);
                base.Dimension = value;
            }
        }

        public GridDimension Capacities
        {
            get => new GridDimension(_data.Capacities[1], _data.Capacities[0]);
            set => _data.Capacities = value.ToArray();
        }

        protected override bool HasLowEntropyProtected => false;
        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected => new Enumerable<IGridCase<T>>(new Enumerator(this));

        public Grid(int columns, int rows, Vector2 delta, Func<T, int[], T> defaultCellValueFactory = null)
            : base(columns, rows, delta)
        {
            _data = new TwoDimensionArray<T>(new T[rows, columns]);
            _defaultCellValueFactory = defaultCellValueFactory;

            _data.Fill(_defaultCellValueFactory ?? ((_, __) => default));
        }

        public Grid(TwoDimensionArray<T> data, Vector2 delta)
            : base(data.GetLength(1), data.GetLength(0), delta)
        {
            _data = data;
        }

        public override void Resize(int[] newLengths, bool keepValues = true, Func<T, int[], T> valueFactory = null)
        {
            _data.Resize(newLengths, keepValues, valueFactory ?? _defaultCellValueFactory);
            base.Dimension = new GridDimension(newLengths[1], newLengths[0]);
        }

        protected override T GetValue(int i, int j) => _data[i, j];
        protected override void SetValue(int i, int j, T value) => _data[i, j] = value;

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