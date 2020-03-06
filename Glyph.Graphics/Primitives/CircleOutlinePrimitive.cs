using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    public class CircleOutlinePrimitive : EllipseOutlinePrimitive
    {
        public CircleOutlinePrimitive(Vector2 center, float radius, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : base(center, radius, radius, 0, angleStart, angleSize, sampling)
        {
        }

        public CircleOutlinePrimitive(Color color, Vector2 center, float radius, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }
    }
}