using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    public class CirclePrimitive : EllipsePrimitive
    {
        public CirclePrimitive(Vector2 center, float radius, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : base(center, radius, radius, 0, angleStart, angleSize, sampling)
        {
        }

        public CirclePrimitive(Color color, Vector2 center, float radius, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, angleStart, angleSize, sampling)
        {
            Colors = new[] { color, color };
        }

        public CirclePrimitive(Color centerColor, Color borderColor, Vector2 center, float radius, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, angleStart, angleSize, sampling)
        {
            Colors = new[] { centerColor, borderColor };
        }
    }
}