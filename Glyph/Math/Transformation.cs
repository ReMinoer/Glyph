using Glyph.Observation;
using Glyph.Observation.Properties;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public class Transformation : ConfigurableNotifyPropertyChanged, IFlipable
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
                _rotation = MathHelper.WrapAngle(value);
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
            _rotation = MathHelper.WrapAngle(rotation);
            _scale = scale;

            RefreshMatrix();
        }

        public Transformation(Transformation transformation)
        {
            _translation = transformation._translation;
            _rotation = transformation._rotation;
            _scale = transformation._scale;

            RefreshMatrix();
        }

        public void Flip(Axes axes)
        {
            double cos = System.Math.Cos(_rotation);
            double sin = System.Math.Sin(_rotation);
            if ((axes & Axes.Horizontal) == Axes.Horizontal)
            {
                _translation.X *= -1;
                cos *= -1;
            }
            if ((axes & Axes.Vertical) == Axes.Vertical)
            {
                _translation.Y *= -1;
                sin *= -1;
            }

            _rotation = (float)System.Math.Atan2(sin, cos);
            RefreshMatrix();
        }

        public void RefreshMatrix(Vector2? translation, float? rotation, float? scale)
        {
            if (translation.HasValue)
                _translation = translation.Value;
            if (rotation.HasValue)
                _rotation = MathHelper.WrapAngle(rotation.Value);
            if (scale.HasValue)
                _scale = scale.Value;
            RefreshMatrix();
        }

        private void RefreshMatrix()
        {
            Matrix = new Matrix3X3(Translation, Rotation, Scale);
        }
    }
}