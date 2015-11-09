using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    static public class RectangleExtension
    {
        static public CenteredRectangle ToCenteredRectangle(this Rectangle value)
        {
            return new CenteredRectangle(value.Center.ToVector2(), value.Width, value.Height);
        }

        static public Rectangle ToStruct(this IRectangle value)
        {
            return new Rectangle((int)value.Left, (int)value.Top, (int)value.Width, (int)value.Height);
        }
    }
}