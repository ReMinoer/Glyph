using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public struct Matrix3X3
    {
        public float M11, M12, M13;
        public float M21, M22, M23;

        public Matrix3X3(Vector2 translation, float rotation, float scale)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);

            M11 = cos * scale;
            M12 = sin;
            M13 = translation.X;
            M21 = -sin;
            M22 = cos * scale;
            M23 = translation.Y;
        }

        public static Vector2 operator *(Matrix3X3 matrix, Vector2 vector)
        {
            return new Vector2
            {
                X = vector.X * matrix.M11 + vector.Y * matrix.M12 + matrix.M13,
                Y = vector.X * matrix.M21 + vector.Y * matrix.M22 + matrix.M23
            };
        }
    }
}