namespace Glyph.Transition
{
    public class TransitionFloat : TransitionVector<float>
    {
        public TransitionFloat()
        {
        }

        public TransitionFloat(ITimingFunction f)
            : base(f)
        {
        }

        public TransitionFloat(float p1X, float p1Y, float p2X, float p2Y)
            : base(p1X, p1Y, p2X, p2Y)
        {
        }

        public TransitionFloat(int steps, bool startInclude)
            : base(steps, startInclude)
        {
        }

        protected override float Add(float a, float b)
        {
            return a + b;
        }

        protected override float Subtract(float a, float b)
        {
            return a - b;
        }

        protected override float Scalar(float a, float b)
        {
            return a * b;
        }

        protected override float Ratio(float a, float b)
        {
            return a / b;
        }

        protected override float Normalize(float a)
        {
            return a.CompareTo(0);
        }
    }
}