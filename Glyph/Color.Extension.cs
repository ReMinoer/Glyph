using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class ColorExtension
    {
        static public float GetOpacity(this Color color)
        {
            return (float)color.A / 255;
        }

        static public Color WithOpacity(this Color color, float alpha)
        {
            return new Color(color, alpha);
        }
    }
}