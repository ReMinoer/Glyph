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
        public Quad Shape
        {
            get
            {
                var shape = new TopLeftRectangle(Vector2.Zero, Delta * Dimension);
                if (Transformation != null)
                    return Transformation.Transform(shape);

                return shape;
            }
        }

        public bool IsVoid => (Dimension.Columns == 0 && Dimension.Rows == 0) || Delta == Vector2.Zero;
        public TopLeftRectangle BoundingBox => Shape.BoundingBox;
        public Vector2 Center => Shape.Center;

        int IArrayDefinition.Rank => 2;
        int IArrayDefinition.GetLowerBound(int dimension) => 0;
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
            Vector2 worldPoint = Delta.Integrate(gridPoint);

            if (Transformation != null)
                return Transformation.Transform(worldPoint);

            return worldPoint;
        }

        public Point ToGridPoint(Vector2 worldPoint)
        {
            if (Transformation != null)
                worldPoint = Transformation.InverseTransform(worldPoint);

            return Delta.Discretize(worldPoint); 
        }

        protected bool IsNotifying => ArrayChanged != null;
        protected void NotifyArrayChanged(ArrayChangedEventArgs e) => ArrayChanged?.Invoke(this, e);
    }

    public class Grid<T> : GridBase<T>
    {
        private readonly TwoDimensionArray<T> _data;
        private readonly Func<T, int[], T> _defaultCellValueFactory;

        public override sealed GridDimension Dimension
        {
            get => base.Dimension;
            set => Resize(value.ToArray());
        }

        public GridDimension Capacities
        {
            get => new GridDimension(_data.Capacities[1], _data.Capacities[0]);
            set => _data.Capacities = value.ToArray();
        }

        protected override bool HasLowEntropyProtected => false;
        protected override IEnumerable<IGridCase<T>> SignificantCasesProtected => new Enumerable<IGridCase<T>>(new Enumerator(this));

        public Grid(int columns, int rows, Vector2 delta, Func<T, int[], T> defaultCellValueFactory = null)
            : this(new TwoDimensionArray<T>(new T[rows, columns]), delta, defaultCellValueFactory)
        {
            _data.Fill(_defaultCellValueFactory ?? ((_, __) => default));
        }

        public Grid(TwoDimensionArray<T> data, Vector2 delta, Func<T, int[], T> defaultCellValueFactory = null)
            : base(data.Lengths[1], data.Lengths[0], delta)
        {
            _data = data;
            _defaultCellValueFactory = defaultCellValueFactory;

            _data.ArrayChanged += OnArrayChanged;
        }

        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            if (e.Action == ArrayChangedAction.Resize)
                base.Dimension = new GridDimension(e.NewLengths[1], e.NewLengths[0]);
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