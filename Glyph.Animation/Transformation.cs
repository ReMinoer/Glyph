using System;
using Microsoft.Xna.Framework;

namespace Glyph.Animation
{
    public class Transformation
    {
        public Vector2 Translation { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Matrix3X3 Matrix { get; private set; }

        public static Transformation Identity
        {
            get
            {
                return new Transformation(Vector2.Zero, 0, 1f);
            }
        }

        public Transformation(Vector2 translation, float rotation, float scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;

            RefreshMatrix();
        }

        public void SetTranslation(Vector2 value)
        {
            if (value == Translation)
                return;

            Translation = value;
            RefreshMatrix();
        }

        public void SetRotation(float value)
        {
            float angle = MathHelper.WrapAngle(value);

            if (Math.Abs(angle - Rotation) < float.Epsilon)
                return;

            Rotation = angle;
            RefreshMatrix();
        }

        public void SetScale(float value)
        {
            if (Math.Abs(value - Scale) < float.Epsilon)
                return;

            Scale = value;
            RefreshMatrix();
        }

        private void RefreshMatrix()
        {
            Matrix = new Matrix3X3(Translation, Rotation, Scale);
        }
    }
}