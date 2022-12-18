using System;
using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public class DirtableGrid<T> : IDirtableResizeableGrid<T>
        where T : class, IDirtableGridCell<T>
    {
        private readonly IResizeableGrid<T> _gridImplementation;
        private readonly List<IGridCase<T>> _dirtiedCases;
        private readonly IReadOnlyList<IGridCase<T>> _readOnlyDirtiedCases;

        public T this[int i, int j]
        {
            get => _gridImplementation[i, j];
            set
            {
                T previousValue = this[i, j];
                if (previousValue == value)
                    return;

                if (previousValue != null)
                    UnregisterValue(previousValue);

                _gridImplementation[i, j] = value;

                if (value != null)
                    RegisterValue(i, j, value);

                SetDirty(i, j, value);
            }
        }

        public T this[Point gridPoint]
        {
            get => _gridImplementation[gridPoint];
            set
            {
                T previousValue = this[gridPoint];
                if (previousValue == value)
                    return;

                if (previousValue != null)
                    UnregisterValue(previousValue);

                _gridImplementation[gridPoint] = value;

                if (value != null)
                    RegisterValue(gridPoint, value);

                SetDirty(new GridCase<T>(gridPoint, value));
            }
        }

        public T this[Vector2 worldPoint]
        {
            get => _gridImplementation[worldPoint];
            set
            {
                T previousValue = this[worldPoint];
                if (previousValue == value)
                    return;

                Point gridPoint = ToGridPoint(worldPoint);

                if (previousValue != null)
                    UnregisterValue(previousValue);

                _gridImplementation[worldPoint] = value;

                if (value != null)
                    RegisterValue(gridPoint, value);

                SetDirty(gridPoint, value);
            }
        }

        T IWriteableArray<T>.this[params int[] indexes]
        {
            get => _gridImplementation[indexes];
            set
            {
                T previousValue = _gridImplementation[indexes];
                if (previousValue == value)
                    return;

                if (previousValue != null)
                    UnregisterValue(previousValue);

                _gridImplementation[indexes] = value;

                if (value != null)
                    RegisterValue(indexes[0], indexes[1], value);

                SetDirty(indexes[0], indexes[1], value);
            }
        }

        public GridDimension Dimension
        {
            get => _gridImplementation.Dimension;
            set => _gridImplementation.Dimension = value;
        }

        public Vector2 Delta
        {
            get => _gridImplementation.Delta;
            set => _gridImplementation.Delta = value;
        }

        public bool IsVoid => _gridImplementation.IsVoid;
        public IEnumerable<IGridCase<T>> DirtiedCases => _readOnlyDirtiedCases;
        public TopLeftRectangle BoundingBox => _gridImplementation.BoundingBox;
        public Vector2 Center => _gridImplementation.Center;
        public bool HasLowEntropy => _gridImplementation.HasLowEntropy;
        public IEnumerable<T> Values => _gridImplementation.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => _gridImplementation.SignificantCases;

        public DirtableGrid(IResizeableGrid<T> gridImplementation)
        {
            _gridImplementation = gridImplementation;
            _gridImplementation.ArrayChanged += OnArrayChanged;

            _dirtiedCases = new List<IGridCase<T>>();
            _readOnlyDirtiedCases = _dirtiedCases.AsReadOnly();
            
            this.GetResetIndex(out int i, out int j);
            while (this.MoveIndex(ref i, ref j))
                RegisterValue(i, j, _gridImplementation[i, j]);
        }

        public event ArrayChangedEventHandler ArrayChanged;
        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            ArrayChanged?.Invoke(this, e);
        }

        public event EventHandler Dirtied;
        public event EventHandler DirtyCleaned;
        public event EventHandler<CellsDirtiedEventArgs<T>> CellsDirtied;

        public void CleanDirty()
        {
            _dirtiedCases.Clear();
            DirtyCleaned?.Invoke(this, EventArgs.Empty);
        }

        private void SetDirty(int i, int j, T value) => SetDirty(new GridCase<T>(i, j, value));
        private void SetDirty(Point point, T value) => SetDirty(new GridCase<T>(point, value));
        private void SetDirty(GridCase<T> gridCase)
        {
            _dirtiedCases.Add(gridCase);

            CellsDirtied?.Invoke(this, CellsDirtiedEventArgs<T>.Change(gridCase.Value));
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        public void SetDirty(T value)
        {
            SetDirty(new GridCase<T>(value.GridPoint, value));
        }

        void IDirtable.SetDirty() => SetDirty();
        private void SetDirty()
        {
            this.GetResetIndex(out int i, out int j);
            while (this.MoveIndex(ref i, ref j))
                _dirtiedCases.Add(new GridCase<T>(i, j, this[i, j]));
            
            CellsDirtied?.Invoke(this, CellsDirtiedEventArgs<T>.GlobalChange());
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        private void RegisterValue(int i, int j, T value) => RegisterValue(new Point(j, i), value);
        private void RegisterValue(Point gridPoint, T value)
        {
            value.Grid = this;
            value.GridPoint = gridPoint;
        }

        private void UnregisterValue(T value)
        {
            value.Grid = null;
        }

        public void Resize(int[] newLengths, bool keepValues = true, Func<T, int[], T> valueFactory = null)
        {
            _gridImplementation.Resize(newLengths, keepValues, (previous, indexes) =>
            {
                if (previous != null)
                    UnregisterValue(previous);

                T value = valueFactory?.Invoke(previous, indexes);

                if (value != null)
                    RegisterValue(indexes[0], indexes[1], value);

                return value;
            });

            SetDirty();
        }

        public bool ContainsPoint(Vector2 point) => _gridImplementation.ContainsPoint(point);
        public bool Intersects(Segment segment) => _gridImplementation.Intersects(segment);
        public bool Intersects<T1>(T1 edgedShape) where T1 : IEdgedShape => _gridImplementation.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _gridImplementation.Intersects(circle);
        public Vector2 ToWorldPoint(Point gridPoint) => _gridImplementation.ToWorldPoint(gridPoint);
        public Point ToGridPoint(Vector2 worldPoint) => _gridImplementation.ToGridPoint(worldPoint);
        public IEnumerator<T> GetEnumerator() => _gridImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _gridImplementation.GetEnumerator();

        int IArrayDefinition.Rank => _gridImplementation.Rank;
        int IArrayDefinition.GetLowerBound(int dimension) => _gridImplementation.GetLowerBound(dimension);
        int IArrayDefinition.GetLength(int dimension) => _gridImplementation.GetLength(dimension);
        object IArray.this[params int[] indexes] => _gridImplementation[indexes];
        T IArray<T>.this[params int[] indexes] => _gridImplementation[indexes];
        object ITwoDimensionArray.this[int i, int j] => _gridImplementation[i, j];

        int[] IResizeableArrayDefinition.Lengths
        {
            set => _gridImplementation.Lengths = value;
        }
    }
}