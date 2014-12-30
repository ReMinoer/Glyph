using System;

namespace Glyph
{
    static public class Angle
    {
        static public float ToRadian(float degree)
        {
            float result = (degree / 180) * (float)Math.PI;
            result %= (float)(2 * Math.PI);
            result += (float)(2 * Math.PI);
            result %= (float)(2 * Math.PI);

            return result;
        }

        static public float ToDegree(float radian)
        {
            float result = radian;
            result %= (float)(2 * Math.PI);
            result += (float)(2 * Math.PI);
            result %= (float)(2 * Math.PI);
            result = (result * 180) / (float)Math.PI;

            return result;
        }

        public class Rotation
        {
            public float Value { get; private set; }

            static public Rotation None { get { return new Rotation {Value = 0}; } }

            static public Rotation RotateRight { get { return new Rotation {Value = (float)(3 * Math.PI) / 2}; } }

            static public Rotation RotateLeft { get { return new Rotation {Value = (float)Math.PI / 2}; } }

            static public Rotation RotateOpposite { get { return new Rotation {Value = (float)Math.PI}; } }

            static public implicit operator float(Rotation x)
            {
                return x.Value;
            }
        }
    }
}