using System.Collections.Generic;
using Glyph.Graphics.Primitives.Base;
using Glyph.Graphics.Primitives.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EllipseOutlinePrimitive : OutlinePrimitiveBase
    {
        private Vector2 _center;
        public Vector2 Center
        {
            get => _center;
            set
            {
                _center = value;
                DirtyVertices();
            }
        }

        private float _width;
        public float Width
        {
            get => _width;
            set
            {
                if (_width.EpsilonEquals(value))
                    return;

                _width = value;
                DirtyVertices();
            }
        }

        private float _height;
        public float Height
        {
            get => _height;
            set
            {
                if (_height.EpsilonEquals(value))
                    return;

                _height = value;
                DirtyVertices();
            }
        }

        private float _rotation;
        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation.EpsilonEquals(value))
                    return;

                _rotation = value;
                DirtyVertices();
            }
        }

        private float _angleStart;
        public float AngleStart
        {
            get => _angleStart;
            set
            {
                if (_angleStart.EpsilonEquals(value))
                    return;

                _angleStart = value;
                DirtyVertices();
            }
        }

        private float _angleSize = MathHelper.TwoPi;
        public float AngleSize
        {
            get => _angleSize;
            set
            {
                if (_angleSize.EpsilonEquals(value))
                    return;

                _angleSize = value;
                DirtyVertices();
            }
        }
        
        private int _sampling = DefaultSampling;
        public const int DefaultSampling = 64;
        public int Sampling
        {
            get => _sampling;
            set
            {
                if (_sampling == value)
                    return;

                _sampling = value;
                DirtyVertices();
            }
        }

        private bool Completed => AngleSize >= MathHelper.TwoPi;
        protected override PrimitiveType PrimitiveType => PrimitiveType.LineStrip;

        public EllipseOutlinePrimitive()
        {
        }

        public EllipseOutlinePrimitive(Vector2 center, float radius, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, radius, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseOutlinePrimitive(Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            Center = center;
            Width = width;
            Height = height;
            Rotation = rotation;
            AngleStart = angleStart;
            AngleSize = angleSize;
            Sampling = sampling;
        }

        public EllipseOutlinePrimitive(Color color, Vector2 center, float radius, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, radius, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }

        public EllipseOutlinePrimitive(Color color, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }

        protected override IEnumerable<Vector2> GetRefreshedVertices()
        {
            IEnumerable<Vector2> points = PrimitiveHelpers.GetEllipseOutlinePoints(Center, Width, Height, Rotation, AngleStart, AngleSize, Sampling);
            using (IEnumerator<Vector2> pointEnumerator = points.GetEnumerator())
            {
                if (!pointEnumerator.MoveNext())
                    yield break;

                Vector2 firstPoint = pointEnumerator.Current;

                yield return firstPoint;
                while (pointEnumerator.MoveNext())
                    yield return pointEnumerator.Current;
                
                if (Completed)
                    yield return firstPoint;
            }
        }
        
        protected override int GetRefreshedVertexCount()
        {
            int count = PrimitiveHelpers.GetEllipseOutlinePointsCount(AngleSize, Sampling);
            return Completed ? count + 1 : count;
        }

        protected override IEnumerable<ushort> GetRefreshedIndices() => null;
        protected override int GetRefreshedIndexCount() => 0;
    }
}