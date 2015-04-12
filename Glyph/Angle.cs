using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    static public class Angle
    {
        static public float ToRadian(float degree)
        {
            float result = (degree / 180) * MathHelper.Pi;
            result %= MathHelper.TwoPi;
            result += MathHelper.TwoPi;
            result %= MathHelper.TwoPi;
            return result;
        }

        static public float ToDegree(float radian)
        {
            float result = radian;
            result %= MathHelper.TwoPi;
            result += MathHelper.TwoPi;
            result %= MathHelper.TwoPi;
            result = (result * 180) / MathHelper.Pi;

            return result;
        }

        public class Rotation
        {
            public float Value { get; private set; }

            static public Rotation None
            {
                get { return new Rotation {Value = 0}; }
            }

            static public Rotation RotateRight
            {
                get { return new Rotation { Value = 3 * MathHelper.PiOver2 }; }
            }

            static public Rotation RotateLeft
            {
                get { return new Rotation { Value = MathHelper.PiOver2 }; }
            }

            static public Rotation RotateOpposite
            {
                get { return new Rotation { Value = MathHelper.Pi }; }
            }

            static public implicit operator float(Rotation x)
            {
                return x.Value;
            }
        }
    }
}