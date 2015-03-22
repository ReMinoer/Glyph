using System;

namespace Glyph.Transition
{
    public class BezierFunction : ITimingFunction
    {
        public float P1X { get; private set; }
        public float P1Y { get; private set; }
        public float P2X { get; private set; }
        public float P2Y { get; private set; }

        static public BezierFunction Linear
        {
            get { return new BezierFunction(0, 0, 1, 1); }
        }

        static public BezierFunction Ease
        {
            get { return new BezierFunction(0.25f, 0.1f, 0.25f, 1f); }
        }

        static public BezierFunction EaseIn
        {
            get { return new BezierFunction(0.42f, 0f, 1f, 1f); }
        }

        static public BezierFunction EaseOut
        {
            get { return new BezierFunction(0f, 0f, 0.58f, 1f); }
        }

        static public BezierFunction EaseInOut
        {
            get { return new BezierFunction(0.42f, 0f, 0.58f, 1f); }
        }

        public BezierFunction(float p1X, float p1Y, float p2X, float p2Y)
        {
            P1X = p1X;
            P1Y = p1Y;
            P2X = p2X;
            P2Y = p2Y;
        }

        public float GetValue(float t)
        {
            return BezierY(FindXfor(t));
        }

        private float BezierX(float t)
        {
            float cx = 3 * P1X;
            float bx = 3 * (P2X - P1X) - cx;
            float ax = 1 - cx - bx;

            return t * (cx + t * (bx + t * ax));
        }

        private float BezierY(float t)
        {
            float cy = 3 * P1Y;
            float by = 3 * (P2Y - P1Y) - cy;
            float ay = 1 - cy - by;

            return t * (cy + t * (by + t * ay));
        }

        private float BezierXDerivative(float t)
        {
            float cx = 3 * P1X;
            float bx = 3 * (P2X - P1X) - cx;
            float ax = 1 - cx - bx;

            return cx + t * (2 * bx + t * 3 * ax);
        }

        private float FindXfor(float t)
        {
            float x = t;

            for (var i = 0; i < 5; i++)
            {
                float z = BezierX(x) - t;
                if (Math.Abs(z) < 1e-3)
                    break;

                x = x - z / BezierXDerivative(x);
            }

            return x;
        }
    }
}