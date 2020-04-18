using System;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public struct RangeArea : IRange<float>, IArea
    {
        private float _min;
        private float _max;

        public Axis Axis { get; set; }
        public bool IsVoid => Min.EpsilonEquals(Max);

        public float Min
        {
            get => _min;
            set
            {
                if (value.CompareTo(_max) > 0)
                    throw new ArgumentOutOfRangeException();

                _min = value;
            }
        }

        public float Max
        {
            get => _max;
            set
            {
                if (value.CompareTo(_min) < 0)
                    throw new ArgumentOutOfRangeException();

                _max = value;
            }
        }

        public TopLeftRectangle BoundingBox
        {
            get
            {
                switch (Axis)
                {
                    case Axis.Horizontal:
                        return new TopLeftRectangle(Min, float.MinValue, Max, 2 * float.MaxValue);
                    case Axis.Vertical:
                        return new TopLeftRectangle(float.MinValue, Min, 2 * float.MaxValue, Max);
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public RangeArea(Axis axis, float min, float max)
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException();

            Axis = axis;
            _min = min;
            _max = max;
        }

        public void Set(float min, float max)
        {
            if (min.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException();

            _min = min;
            _max = max;
        }

        public bool Contains(float value)
        {
            return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
        }

        public bool ContainsPoint(Vector2 point) => Contains(point.Coordinate(Axis));
        public bool Intersects(IRange<float> range) => RangeUtils.Intersects(this, range);

        public bool Intersects(Segment segment) => IntersectionUtils.Intersects(this, segment);
        public bool Intersects<T>(T edgedShape) where T : IEdgedShape => IntersectionUtils.Intersects(this, edgedShape);
        public bool Intersects(Circle circle) => IntersectionUtils.Intersects(this, circle);
    }
}