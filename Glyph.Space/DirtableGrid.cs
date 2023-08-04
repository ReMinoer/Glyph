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

                _gridImplementation[i, j] = value;
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

                _gridImplementation[gridPoint] = value;
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

                _gridImplementation[worldPoint] = value;
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

                _gridImplementation[indexes] = value;
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
                RegisterValue(i, j);
        }

        public event ArrayChangedEventHandler ArrayChanged;
        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            ArrayChanged?.Invoke(this, e);

            switch (e.Action)
            {
                case ArrayChangedAction.Replace:
                {
                    foreach (T oldValue in e.OldValues)
                        UnregisterValue(oldValue);

                    e.NewRange.GetResetIndex(out int i, out int j);
                    while (e.NewRange.MoveIndex(ref i, ref j))
                    {
                        RegisterValue(i, j);
                        SetDirty(i, j);
                    }
                    break;
                }
                case ArrayChangedAction.Resize:
                {
                    // TODO: Unregister values out of new lengths

                    this.GetResetIndex(out int i, out int j);
                    while (this.MoveIndex(ref i, ref j))
                        RegisterValue(i, j);

                    SetDirty();
                    break;
                }
                case ArrayChangedAction.Add:
                {
                    e.NewRange.GetResetIndex(out int i, out int j);
                    while (e.NewRange.MoveIndex(ref i, ref j))
                    {
                        RegisterValue(i, j);
                        SetDirty(i, j);
                    }

                    SetDirty();
                    break;
                }
                case ArrayChangedAction.Remove:
                {
                    foreach (T oldValue in e.OldValues)
                        UnregisterValue(oldValue);

                    SetDirty();
                    break;
                }
                case ArrayChangedAction.Move:
                {
                    SetDirty();
                    break;
                }
                default:
                    throw new NotSupportedException();
            }
        }

        public event EventHandler Dirtied;
        public event EventHandler DirtyCleaned;
        public event EventHandler<CellsDirtiedEventArgs<T>> CellsDirtied;

        public void CleanDirty()
        {
            _dirtiedCases.Clear();
            DirtyCleaned?.Invoke(this, EventArgs.Empty);
        }

        public void SetDirty(T value) => SetDirty(new GridCase<T>(value.GridPoint, value));
        private void SetDirty(int i, int j) => SetDirty(new GridCase<T>(i, j, this[i, j]));
        private void SetDirty(GridCase<T> gridCase)
        {
            _dirtiedCases.Add(gridCase);

            CellsDirtied?.Invoke(this, CellsDirtiedEventArgs<T>.Change(gridCase.Value));
            Dirtied?.Invoke(this, EventArgs.Empty);
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

        private void RegisterValue(int i, int j) => RegisterValue(new Point(j, i), this[i, j]);
        private void RegisterValue(Point gridPoint, T value)
        {
            value.Grid = this;
            value.GridPoint = gridPoint;
        }

        private void UnregisterValue(T value)
        {
            value.Grid = null;
        }

        public void Resize(int[] newLengths, bool keepValues = true, Func<T, int[], T> valueFactory = null) => _gridImplementation.Resize(newLengths, keepValues, valueFactory);
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