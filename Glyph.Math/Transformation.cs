using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public class Transformation
    {
        private Vector2 _translation;
        private float _rotation;
        private float _scale;
        public Matrix3X3 Matrix { get; private set; }

        public Vector2 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                RefreshMatrix();
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = Microsoft.Xna.Framework.MathHelper.WrapAngle(value);
                RefreshMatrix();
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                RefreshMatrix();
            }
        }

        static public Transformation Identity
        {
            get { return new Transformation(Vector2.Zero, 0, 1f); }
        }

        public Transformation(Vector2 translation, float rotation, float scale)
        {
            _translation = translation;
            _rotation = Microsoft.Xna.Framework.MathHelper.WrapAngle(rotation);
            _scale = scale;

            RefreshMatrix();
        }

        private void RefreshMatrix()
        {
            Matrix = new Matrix3X3(Translation, Rotation, Scale);
        }
    }
}