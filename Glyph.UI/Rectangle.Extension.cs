using Microsoft.Xna.Framework;

namespace Glyph.UI
{
    static public class RectangleExtension
    {
        static public Rectangle Padding(this Rectangle value, Padding padding)
        {
            return new Rectangle
            {
                X = value.X - padding.Left,
                Y = value.Y - padding.Top,
                Width = value.Width + padding.Left + padding.Right,
                Height = value.Height + padding.Top + padding.Bottom
            };
        }
    }
}