using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    static public class GridExtension
    {
        static public bool ContainsPoint(this IGrid grid, Point gridPoint)
        {
            return grid.Dimension.ContainsPoint(gridPoint);
        }

        static public bool ContainsPoint(this IGrid grid, int i, int j)
        {
            return grid.Dimension.ContainsPoint(new Point(j, i));
        }

        static public bool ContainsPoint(this IGrid grid, int[] indexes)
        {
            return grid.Dimension.ContainsPoint(new Point(indexes[0], indexes[1]));
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

        static public IEnumerable<int[]> IndexIntersection(this IGrid grid, Segment segment, Func<int, int, bool> cellSelector = null)
            => grid.IndexIntersection(IntersectionUtils.Intersects, segment, cellSelector);
        static public IEnumerable<int[]> IndexIntersection(this IGrid grid, Circle circle, Func<int, int, bool> cellSelector = null)
            => grid.IndexIntersection(IntersectionUtils.Intersects, circle, cellSelector);
        static public IEnumerable<int[]> IndexIntersection<T>(this IGrid grid, T edgedShape, Func<int, int, bool> cellSelector = null) where T : IEdgedShape
            => grid.IndexIntersection(IntersectionUtils.Intersects, edgedShape, cellSelector);

        static public IEnumerable<int[]> IndexIntersection<TOther>(
            this IGrid grid, IntersectionDelegate<TopLeftRectangle, TOther> intersectionDelegate, TOther other, Func<int, int, bool> cellSelector = null)
            where TOther : IShape
        {
            // TODO: Use better grid-shape intersection algorithm
            Rectangle gridBoundingBox = MathUtils.GetBoundingBox(other.BoundingBox.Vertices.Select(grid.ToGridPoint)).ClampToRectangle(grid.IndexesBounds());

            int[] indexes = new int[2];
            for (indexes[0] = gridBoundingBox.Y; indexes[0] <= gridBoundingBox.Bottom; indexes[0]++)
                for (indexes[1] = gridBoundingBox.X; indexes[1] <= gridBoundingBox.Right; indexes[1]++)
                {
                    if (!(cellSelector?.Invoke(indexes[0], indexes[1]) ?? true))
                        continue;

                    var cellRectangle = new TopLeftRectangle(grid.ToWorldPoint(indexes), grid.Delta);
                    if (intersectionDelegate(cellRectangle, other))
                        yield return indexes;
                }
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
}