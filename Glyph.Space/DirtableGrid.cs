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
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                _isDirty = value;

                if (_isDirty)
                    Dirtied?.Invoke();
            }
        }

        public T this[int i, int j]
        {
            get => _gridImplementation[i, j];
            set
            {
                _gridImplementation[i, j] = value;

                value.Dirtied += () => IsDirty = true;
                _dirtiedCases.Add(new GridCase<T>(i, j, value));
                IsDirty = true;
            }
        }

        public T this[Point gridPoint]
        {
            get => _gridImplementation[gridPoint];
            set
            {
                _gridImplementation[gridPoint] = value;

                value.Dirtied += () => IsDirty = true;
                _dirtiedCases.Add(new GridCase<T>(gridPoint, value));
                IsDirty = true;
            }
        }

        public T this[Vector2 worldPoint]
        {
            get => _gridImplementation[worldPoint];
            set
            {
                _gridImplementation[worldPoint] = value;

                value.Dirtied += () => IsDirty = true;
                _dirtiedCases.Add(new GridCase<T>(ToGridPoint(worldPoint), value));
                IsDirty = true;
            }
        }

        public T this[IGridPositionable gridPositionable]
        {
            get => _gridImplementation[gridPositionable];
            set
            {
                _gridImplementation[gridPositionable] = value;

                value.Dirtied += () => IsDirty = true;
                _dirtiedCases.Add(new GridCase<T>(gridPositionable.GridPosition, value));
                IsDirty = true;
            }
        }
        public bool IsVoid => _gridImplementation.IsVoid;
        public IEnumerable<IGridCase<T>> DirtiedCases => _readOnlyDirtiedCases;
        public TopLeftRectangle BoundingBox => _gridImplementation.BoundingBox;
        public Vector2 Center => _gridImplementation.Center;
        public GridDimension Dimension => _gridImplementation.Dimension;
        public Rectangle Bounds => _gridImplementation.Bounds;
        public Vector2 Delta => _gridImplementation.Delta;
        public bool HasLowEntropy => _gridImplementation.HasLowEntropy;
        public IEnumerable<T> Values => _gridImplementation.Values;
        public IEnumerable<IGridCase<T>> SignificantCases => _gridImplementation.SignificantCases;
        
        T IGrid<T>.this[Point gridPoint] => ((IGrid<T>)_gridImplementation)[gridPoint];
        T IGrid<T>.this[Vector2 worldPoint] => ((IGrid<T>)_gridImplementation)[worldPoint];
        T IGrid<T>.this[IGridPositionable gridPositionable] => ((IGrid<T>)_gridImplementation)[gridPositionable];
        
        public event Action Dirtied;
        public event ArrayChangedEventHandler ArrayChanged;

        public DirtableGrid(IWriteableGrid<T> gridImplementation)
        {
            _gridImplementation = gridImplementation;
            _gridImplementation.ArrayChanged += OnArrayChanged;

            foreach (T item in _gridImplementation)
                item.Dirtied += () => IsDirty = true;

            _dirtiedCases = new List<IGridCase<T>>();
            _readOnlyDirtiedCases = _dirtiedCases.AsReadOnly();
        }

        private void OnArrayChanged(object sender, ArrayChangedEventArgs e)
        {
            ArrayChanged?.Invoke(this, e);
        }

        public void Clean()
        {
            IsDirty = false;
            _dirtiedCases.Clear();
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
        T IArray<T>.this[params int[] indexes] => _gridImplementation[indexes];
        T IWriteableArray<T>.this[params int[] indexes]
        {
            get => _gridImplementation[indexes];
            set => _gridImplementation[indexes] = value;
        }
    }
}