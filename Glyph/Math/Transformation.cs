﻿using System;
using Glyph.Observation.Properties;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public class Transformation : ConfigurableNotifyPropertyChanged, ITransformation, IEquatable<ITransformation>
    {
        private Vector2 _translation;
        private float _rotation;
        private float _scale;
        public Matrix3X3 Matrix { get; private set; }

        public Vector2 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                RefreshMatrix();
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = MathHelper.WrapAngle(value);
                RefreshMatrix();
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                RefreshMatrix();
            }
        }

        public event EventHandler TransformationChanged;
        static public Transformation Identity => new Transformation(Vector2.Zero, 0, 1f);
        
        public Transformation()
        {
            _scale = 1f;
            RefreshMatrix();
        }

        public Transformation(Vector2 translation, float rotation, float scale)
        {
            _translation = translation;
            _rotation = MathHelper.WrapAngle(rotation);
            _scale = scale;

            RefreshMatrix();
        }

        public Transformation(ITransformation transformation)
        {
            _translation = transformation.Translation;
            _rotation = transformation.Rotation;
            _scale = transformation.Scale;

            RefreshMatrix();
        }

        public Vector2 Transform(Vector2 position) => Matrix * position;
        public Vector2 InverseTransform(Vector2 position) => Matrix.Inverse * position;

        public ITransformation Transform(ITransformation transformation)
        {
            Vector2 translation = transformation.Translation;
            float rotation = transformation.Rotation;
            float scale = transformation.Scale;

            translation = Matrix * translation;
            rotation = MathHelper.WrapAngle(rotation + Rotation);
            scale *= Scale;

            return new Transformation(translation, rotation, scale);
        }

        public ITransformation InverseTransform(ITransformation transformation)
        {
            Vector2 translation = transformation.Translation;
            float rotation = transformation.Rotation;
            float scale = transformation.Scale;

            translation = Matrix.Inverse * translation;
            rotation = MathHelper.WrapAngle(rotation - Rotation);
            scale /= Scale;

            return new Transformation(translation, rotation, scale);
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

        public bool RefreshMatrix(Vector2? translation, float? rotation, float? scale)
        {
            if ((translation?.Equals(_translation) ?? true)
                && (rotation?.EpsilonEquals(_rotation) ?? true)
                && (scale?.EpsilonEquals(_scale) ?? true))
                return false;

            if (translation.HasValue)
                _translation = translation.Value;
            if (rotation.HasValue)
                _rotation = MathHelper.WrapAngle(rotation.Value);
            if (scale.HasValue)
                _scale = scale.Value;

            RefreshMatrix();
            return true;
        }

        private void RefreshMatrix()
        {
            Matrix = new Matrix3X3Trs(Translation, Rotation, Scale);
            TransformationChanged?.Invoke(this, EventArgs.Empty);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (GetType() != obj.GetType())
                return false;
            
            return EqualsInternal((Transformation)obj);
        }

        public bool Equals(ITransformation other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return EqualsInternal(other);
        }

        public bool EqualsInternal(ITransformation other)
        {
            return _translation.EpsilonEquals(other.Translation)
                   && _rotation.EpsilonEquals(other.Rotation)
                   && _scale.EpsilonEquals(other.Scale)
                   && Matrix.Equals(other.Matrix);
        }
    }
}