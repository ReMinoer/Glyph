using System;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

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

        static public Vector2 ToWorldPoint(this IGrid grid, int i, int j)
        {
            return grid.ToWorldPoint(new Point(j, i));
        }

        static public TopLeftRectangle ToWorldRange(this IGrid grid, int x, int y, int width, int height)
        {
            return grid.ToWorldRange(new Rectangle(x, y, width, height));
        }

        static public TopLeftRectangle ToWorldRange(this IGrid grid, Rectangle rectangle)
        {
            return new TopLeftRectangle(grid.ToWorldPoint(rectangle.Location), grid.Delta.Integrate(rectangle.Size));
        }

        static public Rectangle ToGridRange(this IGrid grid, TopLeftRectangle rectangle)
        {
            Point position = grid.ToGridPoint(rectangle.Position);
            return new Rectangle(position, grid.ToGridPoint(rectangle.P3) - position);
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