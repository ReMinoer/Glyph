using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    public class CirclePrimitive : EllipsePrimitive
    {
        public CirclePrimitive(Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : base(center, radius, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
        }

        public CirclePrimitive(Color color, Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color, color };
        }

        public CirclePrimitive(Color centerColor, Color borderColor, Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { centerColor, borderColor };
        }
    }
}