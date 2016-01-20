using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public struct Matrix3X3
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        public Matrix3X3 Inverse
        {
            get
            {
                var result = new Matrix3X3();

                float determinant = M11 * (M22 * M33 - M23 * M32)
                                    - M12 * (M21 * M33 - M23 * M31)
                                    + M13 * (M21 * M32 - M22 * M31);

                float invdet = 1 / determinant;

                result.M11 = invdet * (M22 * M33 - M23 * M32);
                result.M21 = -invdet * (M12 * M33 - M13 * M32);
                result.M31 = invdet * (M12 * M23 - M13 * M22);
                result.M12 = -invdet * (M21 * M33 - M23 * M31);
                result.M22 = invdet * (M11 * M33 - M13 * M31);
                result.M32 = -invdet * (M11 * M23 - M21 * M13);
                result.M13 = invdet * (M21 * M32 - M31 * M22);
                result.M23 = -invdet * (M11 * M32 - M31 * M12);
                result.M33 = invdet * (M11 * M22 - M21 * M12);

                return result;
            }
        }

        public Matrix3X3(Vector2 translation, float rotation, float scale)
        {
            float cos = (float)System.Math.Cos(rotation);
            float sin = (float)System.Math.Sin(rotation);

            M11 = cos * scale;
            M12 = sin;
            M13 = translation.X;
            M21 = -sin;
            M22 = cos * scale;
            M23 = translation.Y;
            M31 = 0;
            M32 = 0;
            M33 = 1;
        }

        public static Vector2 operator*(Matrix3X3 matrix, Vector2 vector)
        {
            return new Vector2
            {
                X = vector.X * matrix.M11 + vector.Y * matrix.M12 + matrix.M13,
                Y = vector.X * matrix.M21 + vector.Y * matrix.M22 + matrix.M23
            };
        }

        public static Matrix3X3 operator*(Matrix3X3 matrixA, Matrix3X3 matrixB)
        {
            return new Matrix3X3
            {
                M11 = (((matrixA.M11 * matrixB.M11) + (matrixA.M12 * matrixB.M21)) + (matrixA.M13 * matrixB.M31)),
                M12 = (((matrixA.M11 * matrixB.M12) + (matrixA.M12 * matrixB.M22)) + (matrixA.M13 * matrixB.M32)),
                M13 = (((matrixA.M11 * matrixB.M13) + (matrixA.M12 * matrixB.M23)) + (matrixA.M13 * matrixB.M33)),
                M21 = (((matrixA.M21 * matrixB.M11) + (matrixA.M22 * matrixB.M21)) + (matrixA.M23 * matrixB.M31)),
                M22 = (((matrixA.M21 * matrixB.M12) + (matrixA.M22 * matrixB.M22)) + (matrixA.M23 * matrixB.M32)),
                M23 = (((matrixA.M21 * matrixB.M13) + (matrixA.M22 * matrixB.M23)) + (matrixA.M23 * matrixB.M33)),
                M31 = (((matrixA.M31 * matrixB.M11) + (matrixA.M32 * matrixB.M21)) + (matrixA.M33 * matrixB.M31)),
                M32 = (((matrixA.M31 * matrixB.M12) + (matrixA.M32 * matrixB.M22)) + (matrixA.M33 * matrixB.M32)),
                M33 = (((matrixA.M31 * matrixB.M13) + (matrixA.M32 * matrixB.M23)) + (matrixA.M33 * matrixB.M33))
            };
        }
    }
}