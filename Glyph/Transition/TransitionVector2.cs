using Glyph.Transition;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public class TransitionVector2 : TransitionVector<Vector2>
    {
        public TransitionVector2() {}

        public TransitionVector2(ITimingFunction f)
            : base(f) {}

        public TransitionVector2(float p1X, float p1Y, float p2X, float p2Y)
            : base(p1X, p1Y, p2X, p2Y) {}

        public TransitionVector2(int steps, bool startInclude)
            : base(steps, startInclude) {}

        protected override Vector2 Add(Vector2 a, Vector2 b)
        {
            return a + b;
        }

        protected override Vector2 Subtract(Vector2 a, Vector2 b)
        {
            return a - b;
        }

        protected override Vector2 Scalar(Vector2 a, float b)
        {
            return a * b;
        }

        protected override float Ratio(Vector2 a, Vector2 b)
        {
            return a.Length() / b.Length();
        }

        protected override Vector2 Normalize(Vector2 a)
        {
            return Vector2.Normalize(a);
        }
    }
}