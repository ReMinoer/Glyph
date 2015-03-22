using Microsoft.Xna.Framework;

namespace Glyph.Transition
{
    public class TransitionVector3 : TransitionVector<Vector3>
    {
        public TransitionVector3()
        {
        }

        public TransitionVector3(ITimingFunction f)
            : base(f)
        {
        }

        public TransitionVector3(float p1X, float p1Y, float p2X, float p2Y)
            : base(p1X, p1Y, p2X, p2Y)
        {
        }

        public TransitionVector3(int steps, bool startInclude)
            : base(steps, startInclude)
        {
        }

        protected override Vector3 Add(Vector3 a, Vector3 b)
        {
            return a + b;
        }

        protected override Vector3 Subtract(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        protected override Vector3 Scalar(Vector3 a, float b)
        {
            return a * b;
        }

        protected override float Ratio(Vector3 a, Vector3 b)
        {
            return a.Length() / b.Length();
        }

        protected override Vector3 Normalize(Vector3 a)
        {
            return Vector3.Normalize(a);
        }
    }
}