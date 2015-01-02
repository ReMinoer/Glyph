using Microsoft.Xna.Framework;

namespace Glyph.Transition
{
    public class TransitionVector4 : TransitionVector<Vector4>
    {
        public TransitionVector4() {}

        public TransitionVector4(ITimingFunction f)
            : base(f) {}

        public TransitionVector4(float p1X, float p1Y, float p2X, float p2Y)
            : base(p1X, p1Y, p2X, p2Y) {}

        public TransitionVector4(int steps, bool startInclude)
            : base(steps, startInclude) {}

        protected override Vector4 Add(Vector4 a, Vector4 b)
        {
            return a + b;
        }

        protected override Vector4 Subtract(Vector4 a, Vector4 b)
        {
            return a - b;
        }

        protected override Vector4 Scalar(Vector4 a, float b)
        {
            return a * b;
        }

        protected override float Ratio(Vector4 a, Vector4 b)
        {
            return a.Length() / b.Length();
        }

        protected override Vector4 Normalize(Vector4 a)
        {
            return Vector4.Normalize(a);
        }
    }
}