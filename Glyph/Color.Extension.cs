using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class ColorExtension
    {
        static public float GetOpacity(this Color color)
        {
            return MathHelper.Lerp(0, 1, (float)color.A / 255);
        }

        static public Color SetOpacity(this Color color, float alpha)
        {
            return color * alpha;
        }
    }
}