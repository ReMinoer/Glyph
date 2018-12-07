using System;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public struct Matrix3X3 : IEquatable<Matrix3X3>
    {
        public float M11, M21, M31;
        public float M12, M22, M32;
        public float M13, M23, M33;

        public Matrix3X3 Inverse
        {
            get
            {
                var result = new Matrix3X3();

                float determinant = M11 * (M22 * M33 - M32 * M23)
                                    - M21 * (M12 * M33 - M32 * M13)
                                    + M31 * (M12 * M23 - M22 * M13);

                float invdet = 1 / determinant;

                // (1 / det(A)) * (cofactor matrix of A)T
                result.M11 = invdet * (M22 * M33 - M32 * M23);
                result.M21 = invdet * (M21 * M33 - M31 * M23) * -1;
                result.M31 = invdet * (M21 * M32 - M31 * M22);
                result.M12 = invdet * (M12 * M33 - M32 * M13) * -1;
                result.M22 = invdet * (M11 * M33 - M31 * M13);
                result.M32 = invdet * (M11 * M32 - M31 * M12) * -1;
                result.M13 = invdet * (M12 * M23 - M22 * M13);
                result.M23 = invdet * (M11 * M23 - M21 * M13) * -1;
                result.M33 = invdet * (M11 * M22 - M21 * M12);

                return result;
            }
        }

        static public Matrix3X3 Identity => new Matrix3X3(1, 0, 0, 0, 1, 0, 0, 0, 1);

        public Matrix3X3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        static public Vector2 operator*(Matrix3X3 matrix, Vector2 vector)
        {
            return new Vector2
            {
                X = vector.X * matrix.M11 + vector.Y * matrix.M21 + matrix.M31,
                Y = vector.X * matrix.M12 + vector.Y * matrix.M22 + matrix.M32
            };
        }

        static public Matrix3X3 operator*(Matrix3X3 matrixA, Matrix3X3 matrixB)
        {
            return new Matrix3X3
            {
                M11 = (((matrixA.M11 * matrixB.M11) + (matrixA.M21 * matrixB.M12)) + (matrixA.M31 * matrixB.M13)),
                M12 = (((matrixA.M12 * matrixB.M11) + (matrixA.M22 * matrixB.M12)) + (matrixA.M32 * matrixB.M13)),
                M13 = (((matrixA.M13 * matrixB.M11) + (matrixA.M23 * matrixB.M12)) + (matrixA.M33 * matrixB.M13)),
                M21 = (((matrixA.M11 * matrixB.M21) + (matrixA.M21 * matrixB.M22)) + (matrixA.M31 * matrixB.M23)),
                M22 = (((matrixA.M12 * matrixB.M21) + (matrixA.M22 * matrixB.M22)) + (matrixA.M32 * matrixB.M23)),
                M23 = (((matrixA.M13 * matrixB.M21) + (matrixA.M23 * matrixB.M22)) + (matrixA.M33 * matrixB.M23)),
                M31 = (((matrixA.M11 * matrixB.M31) + (matrixA.M21 * matrixB.M32)) + (matrixA.M31 * matrixB.M33)),
                M32 = (((matrixA.M12 * matrixB.M31) + (matrixA.M22 * matrixB.M32)) + (matrixA.M32 * matrixB.M33)),
                M33 = (((matrixA.M13 * matrixB.M31) + (matrixA.M23 * matrixB.M32)) + (matrixA.M33 * matrixB.M33))
            };
        }

        static public implicit operator Matrix(Matrix3X3 m)
        {
            return new Matrix(m.M11, m.M12, 0, m.M13, m.M21, m.M22, 0, m.M23, 0, 0, 1, 0, m.M31, m.M32, 0, m.M33);
        }

        public override string ToString() => $"({M11} {M12} {M13}) ({M21} {M22} {M23}) ({M31} {M32} {M33})";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Matrix3X3 && Equals((Matrix3X3)obj);
        }

        public bool Equals(Matrix3X3 other)
        {
            return M11.Equals(other.M11) && M21.Equals(other.M21) && M31.Equals(other.M31)
                && M12.Equals(other.M12) && M22.Equals(other.M22) && M32.Equals(other.M32)
                && M13.Equals(other.M13) && M23.Equals(other.M23) && M33.Equals(other.M33);
        }

        public override int GetHashCode()
        {
            return M11.GetHashCode() ^ M21.GetHashCode() ^ M31.GetHashCode()
                ^ M12.GetHashCode() ^ M22.GetHashCode() ^ M32.GetHashCode()
                ^ M13.GetHashCode() ^ M23.GetHashCode() ^ M33.GetHashCode();
        }
    }
}