using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public class RetypedGrid<TOldValue, TNewValue> : RetypedArray<TOldValue, TNewValue>, IGrid<TNewValue>
    {
        protected readonly IGrid<TOldValue> Grid;

        public bool IsVoid => Grid.IsVoid;
        public TopLeftRectangle BoundingBox => Grid.BoundingBox;
        public Vector2 Center => Grid.Center;

        public GridDimension Dimension => Grid.Dimension;
        public Vector2 Delta => Grid.Delta;
        public ITransformation Transformation
        {
            get => Grid.Transformation;
            set => Grid.Transformation = value;
        }

        public bool HasLowEntropy => Grid.HasLowEntropy;

        public event ArrayChangedEventHandler ArrayChanged
        {
            add => Grid.ArrayChanged += value;
            remove => Grid.ArrayChanged -= value;
        }

        public RetypedGrid(IGrid<TOldValue> grid, Func<TOldValue, TNewValue> getter)
            : base(grid, getter)
        {
            Grid = grid;
        }

        public Vector2 ToWorldPoint(Point gridPoint) => Grid.ToWorldPoint(gridPoint);
        public Point ToGridPoint(Vector2 worldPoint) => Grid.ToGridPoint(worldPoint);

        public bool ContainsPoint(Vector2 point) => Grid.ContainsPoint(point);
        public bool Intersects(Segment segment) => Grid.Intersects(segment);
        public bool Intersects(Circle circle) => Grid.Intersects(circle);
        public bool Intersects<T>(T edgedShape)
            where T : IEdgedShape
        {
            return Grid.Intersects(edgedShape);
        }
        
        public IEnumerable<TNewValue> Values => Grid.Values.Select(Getter);
        public IEnumerable<IGridCase<TNewValue>> SignificantCases => Grid.SignificantCases.Select<IGridCase<TOldValue>, IGridCase<TNewValue>>(x => new GridCase<TNewValue>(x.Point, Getter(x.Value)));
        public TNewValue this[int i, int j] => Getter(Grid[i, j]);
        public TNewValue this[Point gridPoint] => Getter(Grid[gridPoint]);
        public TNewValue this[Vector2 worldPoint] => Getter(Grid[worldPoint]);
        object ITwoDimensionArray.this[int i, int j] => this[i, j];

        public TNewValue[,] ToArray()
        {
            var array = new TNewValue[Dimension.Rows, Dimension.Columns];
            for (int i = 0; i < Dimension.Rows; i++)
                for (int j = 0; j < Dimension.Columns; j++)
                    array[i, j] = this[i, j];

            return array;
        }
    }

    public class RetypedWriteableGrid<TOldValue, TNewValue> : RetypedGrid<TOldValue, TNewValue>, IWriteableGrid<TNewValue>
    {
        protected readonly Action<TOldValue, TNewValue> Setter;

        public RetypedWriteableGrid(IGrid<TOldValue> grid, Func<TOldValue, TNewValue> getter, Action<TOldValue, TNewValue> setter)
            : base(grid, getter)
        {
            Setter = setter;
        }

        new public TNewValue this[params int[] indexes]
        {
            get => base[indexes];
            set => Setter(Grid[indexes], value);
        }

        new public TNewValue this[int i, int j]
        {
            get => base[i, j];
            set => Setter(Grid[i, j], value);
        }

        new public TNewValue this[Point gridPoint]
        {
            get => base[gridPoint];
            set => Setter(Grid[gridPoint], value);
        }

        new public TNewValue this[Vector2 worldPoint]
        {
            get => base[worldPoint];
            set => Setter(Grid[worldPoint], value);
        }
    }
}