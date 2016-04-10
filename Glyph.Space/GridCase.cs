using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public struct GridCase<T> : IGridCase<T>
    {
        public Point Point { get; private set; }
        public T Value { get; private set; }

        public GridCase(Point point, T value)
        {
            Point = point;
            Value = value;
        }
        public GridCase(int i, int j, T value)
        {
            Point = new Point(j, i);
            Value = value;
        }
    }
}