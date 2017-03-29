using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class RectangleExtension
    {
        static public TopLeftRectangle ToFloats(this Rectangle value)
        {
            return new TopLeftRectangle(value.X, value.Y, value.Width, value.Height);
        }

        static public Rectangle ToIntegers(this TopLeftRectangle value)
        {
            return new Rectangle((int)value.Left, (int)value.Top, (int)value.Width, (int)value.Height);
        }

        static public Rectangle ToIntegers(this CenteredRectangle value)
        {
            return new Rectangle((int)value.Left, (int)value.Top, (int)value.Width, (int)value.Height);
        }
    }
}