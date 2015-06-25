using System;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public struct Transformation
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Matrix3X3 Matrix { get; private set; }

        public static Transformation Identity
        {
            get
            {
                return new Transformation {
                    Position = Vector2.Zero,
                    Rotation = 0,
                    Scale = 1f
                };
            }
        }

        public Transformation(Vector2 position, float rotation, float scale)
            : this()
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;

            RefreshMatrix();
        }

        public void SetPosition(Vector2 value)
        {
            if (value == Position)
                return;

            Position = value;
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
            Matrix = new Matrix3X3(Position, Rotation, Scale);
        }
    }
}