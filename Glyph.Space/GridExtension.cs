using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    static public class GridExtension
    {
        static public bool ContainsPoint(this IGrid grid, Point gridPoint)
        {
            return gridPoint.X >= 0 && gridPoint.X < grid.Dimension.Columns
                && gridPoint.Y >= 0 && gridPoint.Y < grid.Dimension.Rows;
        }

        static public bool ContainsPoint(this IGrid grid, int i, int j)
        {
            return grid.ContainsPoint(new Point(j, i));
        }

        static public bool ContainsPoint(this IGrid grid, int[] indexes)
        {
            return grid.ContainsPoint(indexes[0], indexes[1]);
        }

        static public Vector2 ToWorldPoint(this IGrid grid, int i, int j)
        {
            return grid.ToWorldPoint(new Point(j, i));
        }

        static public Vector2 ToWorldPoint(this IGrid grid, int[] indexes)
        {
            return grid.ToWorldPoint(indexes[0], indexes[1]);
        }

        static public Quad ToWorldRange(this IGrid grid, int x, int y, int width, int height)
        {
            return grid.ToWorldRange(new Rectangle(x, y, width, height));
        }

        static public Quad ToWorldRange(this IGrid grid, Rectangle rectangle)
        {
            return new Quad(grid.ToWorldPoint(rectangle.Location), grid.ToWorldPoint(rectangle.P1()), grid.ToWorldPoint(rectangle.P2()));
        }

        static public GridIntersection Intersection(this IGrid grid, Segment segment, Func<int, int, bool> cellSelector = null)
            => grid.Intersection(IntersectionUtils.Intersects, segment, cellSelector);
        static public GridIntersection Intersection(this IGrid grid, Circle circle, Func<int, int, bool> cellSelector = null)
            => grid.Intersection(IntersectionUtils.Intersects, circle, cellSelector);
        static public GridIntersection Intersection<T>(this IGrid grid, T edgedShape, Func<int, int, bool> cellSelector = null) where T : IEdgedShape
            => grid.Intersection(IntersectionUtils.Intersects, edgedShape, cellSelector);

        static public GridIntersection Intersection<TOther>(
            this IGrid grid, IntersectionDelegate<TopLeftRectangle, TOther> intersectionDelegate, TOther other, Func<int, int, bool> cellSelector = null)
            where TOther : IShape
        {
            Rectangle gridBoundingBox = MathUtils.GetBoundingBox(other.BoundingBox.Vertices.Select(grid.ToGridPoint)).ClampToRectangle(grid.IndexesBounds());

            var sequences = new List<GridIntersectionSequence>();
            int sequenceI = 0;
            int sequenceJ = 0;
            int length = 0;

            for (int i = gridBoundingBox.Y; i <= gridBoundingBox.Bottom; i++)
            {
                for (int j = gridBoundingBox.X; j <= gridBoundingBox.Right; j++)
                {
                    if (!(cellSelector?.Invoke(i, j) ?? true))
                        continue;

                    var cellRectangle = new TopLeftRectangle(grid.ToWorldPoint(i, j), grid.Delta);

                    if (intersectionDelegate(cellRectangle, other))
                    {
                        if (length == 0)
                        {
                            sequenceI = i;
                            sequenceJ = j;
                        }

                        length++;
                    }
                    else
                    {
                        sequences.Add(new GridIntersectionSequence(sequenceI, sequenceJ, length));
                        length = 0;
                    }
                }

                sequences.Add(new GridIntersectionSequence(sequenceI, sequenceJ, length));
                length = 0;
            }

            return new GridIntersection(sequences);
        }

        static public Rectangle IndexesBounds(this IGrid grid)
        {
            return new Rectangle(0, 0, grid.Dimension.Columns - 1, grid.Dimension.Rows - 1);
        }

        static public IGrid<TNewValue> Retype<TOldValue, TNewValue>(this IGrid<TOldValue> array, Func<TOldValue, TNewValue> getter)
        {
            return new RetypedGrid<TOldValue, TNewValue>(array, getter);
        }

        static public IWriteableGrid<TNewValue> Retype<TOldValue, TNewValue>(this IWriteableGrid<TOldValue> array, Func<TOldValue, TNewValue> getter, Action<TOldValue, TNewValue> setter)
        {
            return new RetypedWriteableGrid<TOldValue, TNewValue>(array, getter, setter);
        }
    }

    public class GridIntersection
    {
        private readonly IList<GridIntersectionSequence> _sequences;
        public bool IsEmpty => _sequences.Count == 0;

        public GridIntersection(IList<GridIntersectionSequence> orderedSequences)
        {
            _sequences = orderedSequences;
        }

        public IIndexEnumerator GetIndexEnumerator() => new Enumerator(_sequences);

        private class Enumerator : IIndexEnumerator
        {
            private readonly IList<GridIntersectionSequence> _sequencesArray;
            private int _arrayIndex;
            private int _sequenceIndex;

            public Enumerator(IList<GridIntersectionSequence> sequencesArray)
            {
                _sequencesArray = sequencesArray;
            }

            public int[] GetResetIndex()
            {
                _arrayIndex = 0;
                _sequenceIndex = -1;

                if (_sequencesArray.Count == 0)
                    return new[] { 0, -1 };

                return new[] { _sequencesArray[0].I, _sequencesArray[0].J - 1 };
            }

            public bool MoveIndex(int[] indexes)
            {
                if (_sequencesArray.Count == 0)
                    return false;

                _sequenceIndex++;
                if (_sequenceIndex >= _sequencesArray[_arrayIndex].Length)
                {
                    _sequenceIndex = 0;
                    _arrayIndex++;
                    if (_arrayIndex >= _sequencesArray.Count)
                        return false;
                }

                GridIntersectionSequence sequence = _sequencesArray[_arrayIndex];
                indexes[0] = sequence.I;
                indexes[1] = sequence.J + _sequenceIndex;
                return true;
            }
        }
    }

    public struct GridIntersectionSequence : IComparable<GridIntersectionSequence>
    {
        public int I { get; }
        public int J { get; }
        public int Length { get; }

        public GridIntersectionSequence(int i, int j, int length)
        {
            I = i;
            J = j;
            Length = length;
        }

        public int CompareTo(GridIntersectionSequence other)
        {
            int result = I.CompareTo(other.I);
            if (result != 0)
                return result;

            return J.CompareTo(other.J);
        }
    }
}