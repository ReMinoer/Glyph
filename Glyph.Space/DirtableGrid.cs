using System;
using System.Collections;
using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public class DirtableGrid<T> : IDirtableGrid<T>
        where T : class, IDirtable
    {
        private readonly IWriteableGrid<T> _gridImplementation;
        private readonly List<IGridCase<T>> _dirtiedCases;
        private readonly IReadOnlyList<IGridCase<T>> _readOnlyDirtiedCases;
        private readonly EventHandler[,] _dirtyHandlers;

        public T this[int i, int j]
        {
            get => _gridImplementation[i, j];
            set
            {
                T previousValue = this[i, j];
                if (previousValue == value)
                    return;

                if (previousValue != null)
                    UnsubscribeDirty(new GridCase<T>(i, j, previousValue));

                _gridImplementation[i, j] = value;

                if (value != null)
                    SubscribeDirty(new GridCase<T>(i, j, value));

                _dirtiedCases.Add(new GridCase<T>(i, j, value));

                SetDirty(new GridCase<T>(i, j, value));
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
                    UnsubscribeDirty(new GridCase<T>(gridPoint, previousValue));

                _gridImplementation[gridPoint] = value;

                if (value != null)
                    SubscribeDirty(new GridCase<T>(gridPoint, value));

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
                    UnsubscribeDirty(new GridCase<T>(gridPoint, previousValue));

                _gridImplementation[worldPoint] = value;

                if (value != null)
                    SubscribeDirty(new GridCase<T>(gridPoint, value));

                SetDirty(new GridCase<T>(gridPoint, value));
            }
        }

        public T this[IGridPositionable gridPositionable]
        {
            get => _gridImplementation[gridPositionable];
            set
            {
                T previousValue = this[gridPositionable];
                if (previousValue == value)
                    return;

                if (previousValue != null)
                    UnsubscribeDirty(new GridCase<T>(gridPositionable.GridPosition, previousValue));

                _gridImplementation[gridPositionable] = value;

                if (value != null)
                    SubscribeDirty(new GridCase<T>(gridPositionable.GridPosition, value));

                SetDirty(new GridCase<T>(gridPositionable.GridPosition, value));
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
                    UnsubscribeDirty(new GridCase<T>(indexes[0], indexes[1], previousValue));

                _gridImplementation[indexes] = value;

                if (value != null)
                    SubscribeDirty(new GridCase<T>(indexes[0], indexes[1], value));

                SetDirty(new GridCase<T>(indexes[0], indexes[1], value));
            }
        }

        public bool IsVoid => _gridImplementation.IsVoid;
        public IEnumerable<IGridCase<T>> DirtiedCases => _readOnlyDirtiedCases;
        public TopLeftRectangle BoundingBox => _gridImplementation.BoundingBox;
        public Vector2 Center => _gridImplementation.Center;
        public GridDimension Dimension => _gridImplementation.Dimension;
        public Vector2 Delta => _gridImplementation.Delta;
        public bool HasLowEntropy => _gridImplementation.HasLowEntropy;
        public IEnumerable<T> Values => _gridImplementation.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => _gridImplementation.SignificantCases;
        
        T IGrid<T>.this[Point gridPoint] => ((IGrid<T>)_gridImplementation)[gridPoint];
        T IGrid<T>.this[Vector2 worldPoint] => ((IGrid<T>)_gridImplementation)[worldPoint];
        T IGrid<T>.this[IGridPositionable gridPositionable] => ((IGrid<T>)_gridImplementation)[gridPositionable];

        public DirtableGrid(IWriteableGrid<T> gridImplementation)
        {
            _gridImplementation = gridImplementation;
            _gridImplementation.ArrayChanged += OnArrayChanged;

            _dirtiedCases = new List<IGridCase<T>>();
            _readOnlyDirtiedCases = _dirtiedCases.AsReadOnly();

            _dirtyHandlers = new EventHandler[_gridImplementation.Dimension.Rows, _gridImplementation.Dimension.Columns];

            int[] indexes = this.GetResetIndex();
            while (this.MoveIndex(indexes))
                SubscribeDirty(new GridCase<T>(indexes[0], indexes[1], _gridImplementation[indexes]));
        }

        public event ArrayChangedEventHandler ArrayChanged;
        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            ArrayChanged?.Invoke(this, e);
        }

        public event EventHandler Dirtied;
        public event EventHandler DirtyCleaned;

        public void CleanDirty()
        {
            _dirtiedCases.Clear();
            DirtyCleaned?.Invoke(this, EventArgs.Empty);
        }

        private void SetDirty(GridCase<T> gridCase)
        {
            _dirtiedCases.Add(gridCase);
            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        void IDirtable.SetDirty()
        {
            int[] indexes = this.GetResetIndex();
            while (this.MoveIndex(indexes))
                _dirtiedCases.Add(new GridCase<T>(indexes[0], indexes[1], this[indexes[0], indexes[1]]));

            Dirtied?.Invoke(this, EventArgs.Empty);
        }

        private void SubscribeDirty(GridCase<T> gridCase)
        {
            Point point = gridCase.Point;

            gridCase.Value.Dirtied += Handler;
            _dirtyHandlers[point.Y, point.X] = Handler;

            void Handler(object sender, EventArgs e) => SetDirty(new GridCase<T>(point, this[point]));
        }

        private void UnsubscribeDirty(GridCase<T> gridCase)
        {
            Point point = gridCase.Point;

            gridCase.Value.Dirtied -= _dirtyHandlers[point.Y, point.X];
            _dirtyHandlers[point.Y, point.X] = null;
        }

        public bool ContainsPoint(Vector2 point) => _gridImplementation.ContainsPoint(point);
        public bool Intersects(Segment segment) => _gridImplementation.Intersects(segment);
        public bool Intersects<T1>(T1 edgedShape) where T1 : IEdgedShape => _gridImplementation.Intersects(edgedShape);
        public bool Intersects(Circle circle) => _gridImplementation.Intersects(circle);
        public Vector2 ToWorldPoint(int i, int j) => _gridImplementation.ToWorldPoint(i, j);
        public Vector2 ToWorldPoint(Point gridPoint) => _gridImplementation.ToWorldPoint(gridPoint);
        public Vector2 ToWorldPoint(IGridPositionable gridPoint) => _gridImplementation.ToWorldPoint(gridPoint);
        public TopLeftRectangle ToWorldRange(int x, int y, int width, int height) => _gridImplementation.ToWorldRange(x, y, width, height);
        public TopLeftRectangle ToWorldRange(Rectangle rectangle) => _gridImplementation.ToWorldRange(rectangle);
        public Point ToGridPoint(Vector2 worldPoint) => _gridImplementation.ToGridPoint(worldPoint);
        public Rectangle ToGridRange(TopLeftRectangle rectangle) => _gridImplementation.ToGridRange(rectangle);
        public bool ContainsPoint(int i, int j) => _gridImplementation.ContainsPoint(i, j);
        public bool ContainsPoint(Point gridPoint) => _gridImplementation.ContainsPoint(gridPoint);
        public T[][] ToArray() => _gridImplementation.ToArray();
        public IEnumerator<T> GetEnumerator() => _gridImplementation.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _gridImplementation.GetEnumerator();

        int IArray.Rank => _gridImplementation.Rank;
        int IArray.GetLength(int dimension) => _gridImplementation.GetLength(dimension);
        object IArray.this[params int[] indexes] => _gridImplementation[indexes];
        T IArray<T>.this[params int[] indexes] => _gridImplementation[indexes];
    }
}