using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.Meshes.Utils;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Meshes
{
    public class EllipseOutlineMesh : LineProceduralMeshBase
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
        protected override bool IsStrip => true;

        public EllipseOutlineMesh()
        {
        }

        public EllipseOutlineMesh(Vector2 center, float radius, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, radius, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseOutlineMesh(Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            Center = center;
            Width = width;
            Height = height;
            Rotation = rotation;
            AngleStart = angleStart;
            AngleSize = angleSize;
            Sampling = sampling;
        }

        public EllipseOutlineMesh(Color color, Vector2 center, float radius, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, radius, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }

        public EllipseOutlineMesh(Color color, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }

        protected override IEnumerable<Vector2> GetRefreshedVertices()
        {
            IEnumerable<Vector2> points = MeshHelpers.GetEllipseOutlinePoints(Center, Width, Height, Rotation, AngleStart, AngleSize, Sampling);
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
            int count = MeshHelpers.GetEllipseOutlinePointsCount(AngleSize, Sampling);
            return Completed ? count + 1 : count;
        }

        protected override IEnumerable<int> GetRefreshedIndices() => null;
        protected override int GetRefreshedIndexCount() => 0;
    }
}