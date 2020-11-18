using System.Collections.Generic;
using Glyph.Graphics.Meshes.Base;
using Glyph.Graphics.Meshes.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Meshes
{
    public class EllipseMesh : FullyProceduralMeshBase
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

        private float _thickness = float.MaxValue;
        public float Thickness
        {
            get => _thickness;
            set
            {
                if (_thickness.EpsilonEquals(value))
                    return;

                _thickness = value;
                DirtyVertices();
                DirtyIndices();
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
                DirtyIndices();
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
                DirtyIndices();
            }
        }

        public override PrimitiveType Type => PrimitiveType.TriangleList;
        public Color[] CenterColors { get; set; }
        public Color[] BorderColors { get; set; }

        public bool HasInnerVoid => Thickness < Width && Thickness < Height;
        private bool Completed => AngleSize >= MathHelper.TwoPi;

        public EllipseMesh()
        {
        }

        public EllipseMesh(Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, radius, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseMesh(Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            Center = center;
            Width = width;
            Height = height;
            Thickness = thickness;
            Rotation = rotation;
            AngleStart = angleStart;
            AngleSize = angleSize;
            Sampling = sampling;
        }

        public EllipseMesh(Color color, Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(color, color, center, radius, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseMesh(Color color, Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(color, color, center, width, height, thickness, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseMesh(Color centerColor, Color borderColor, Vector2 center, float radius, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(centerColor, borderColor, center, radius, radius, thickness, rotation, angleStart, angleSize, sampling)
        {
        }

        public EllipseMesh(Color centerColor, Color borderColor, Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, thickness, rotation, angleStart, angleSize, sampling)
        {
            CenterColors = new[] { centerColor };
            BorderColors = new[] { borderColor };
        }

        protected override IEnumerable<Vector2> GetRefreshedVertices()
        {
            if (HasInnerVoid)
            {
                foreach (Vector2 point in MeshHelpers.GetEllipseOutlinePoints(Center, Width - Thickness, Height - Thickness, Rotation, AngleStart, AngleSize, Sampling))
                    yield return point;
            }
            else
            {
                yield return Center;
            }

            foreach (Vector2 point in MeshHelpers.GetEllipseOutlinePoints(Center, Width, Height, Rotation, AngleStart, AngleSize, Sampling))
                yield return point;
        }

        protected override IEnumerable<int> GetRefreshedIndices()
        {
            int n = MeshHelpers.GetEllipseOutlinePointsCount(AngleSize, Sampling);
            
            int i;
            if (HasInnerVoid)
            {
                for (i = 0; i < n - 2; i++)
                {
                    yield return (ushort)i;
                    yield return (ushort)(n + i);
                    yield return (ushort)(n + i + 1);
                    yield return (ushort)i;
                    yield return (ushort)(n + i + 1);
                    yield return (ushort)(i + 1);
                }
                
                yield return (ushort)i;
                yield return (ushort)(n + i);
                yield return (ushort)(Completed ? n : n + i + 1);
                yield return (ushort)i;
                yield return (ushort)(Completed ? n : n + i + 1);
                yield return (ushort)(Completed ? 0 : i + 1);
            }
            else
            {
                for (i = 0; i < n - 1; i++)
                {
                    yield return 0;
                    yield return (ushort)(i + 1);
                    yield return (ushort)(i + 2);
                }
                
                yield return 0;
                yield return (ushort)(i + 1);
                yield return (ushort)(Completed ? 1 : i + 2);
            }
        }

        protected override Color GetColor(int i)
        {
            if (HasInnerVoid)
                return i < VertexCount / 2 ? CenterColors[i % CenterColors.Length] : BorderColors[i % BorderColors.Length];
            
            return i == 0 ? CenterColors[0] : BorderColors[i % BorderColors.Length];
        }

        protected override int GetRefreshedVertexCount()
        {
            int pointsCount = MeshHelpers.GetEllipseOutlinePointsCount(AngleSize, Sampling);
            return HasInnerVoid ? pointsCount * 2 : pointsCount + 1;
        }

        protected override int GetRefreshedIndexCount()
        {
            int pointsCount = MeshHelpers.GetEllipseOutlinePointsCount(AngleSize, Sampling);
            return HasInnerVoid ? (pointsCount - 1) * 6 : pointsCount * 3;
        }
    }
}