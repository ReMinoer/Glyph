using System;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public struct Matrix3X3Trs : IEquatable<Matrix3X3Trs>
    {
        public Vector2 Translation { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Matrix3X3Trs(Vector2 translation, float rotation, float scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        static public implicit operator Matrix3X3(Matrix3X3Trs m)
        {
            float scaledCos = (float)System.Math.Cos(m.Rotation) * m.Scale;
            float scaledSin = (float)System.Math.Sin(m.Rotation) * m.Scale;

            float m11 = scaledCos;
            float m12 = -scaledSin;
            float m21 = scaledSin;
            float m22 = scaledCos;
            float m31 = m.Translation.X;
            float m32 = m.Translation.Y;

            return new Matrix3X3(m11, m21, 0, m12, m22, 0, m31, m32, 1);
        }

        public override string ToString() => $"(T:{Translation}, R:{Rotation}, S:{Scale})";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Matrix3X3Trs trs && Equals(trs)
                   || obj is Matrix3X3 matrix && ((Matrix3X3)this).Equals(matrix);
        }

        public bool Equals(Matrix3X3Trs other)
        {
            return Translation.Equals(other.Translation)
                   && Rotation.Equals(other.Rotation)
                   && Scale.Equals(other.Scale);
        }

        public override int GetHashCode()
        {
            return Translation.GetHashCode()
                   ^ Rotation.GetHashCode()
                   ^ Scale.GetHashCode();
        }
    }
}