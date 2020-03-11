using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Graphics.Primitives.Base;
using Glyph.Graphics.Primitives.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EllipsePrimitive : PrimitiveBase
    {
        public const int DefaultSampling = 64;

        public override PrimitiveType PrimitiveType => PrimitiveType.TriangleList;
        
        public override IEnumerable<Vector2> Vertices { get; }
        public override IEnumerable<ushort> Indices { get; }
        public override int VertexCount { get; }
        public override int IndexCount { get; }
        public override sealed Color[] Colors { get; set; }
        private readonly bool _hasInnerSize;

        public EllipsePrimitive(Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            Vector2[] vertices;
            ushort[] indices;
            
            int vertexIndex = 0;
            int pointsCount = PrimitiveHelpers.GetEllipseOutlinePointsCount(angleSize, sampling, out bool completed);
            _hasInnerSize = thickness < width && thickness < height;

            if (_hasInnerSize)
            {
                vertices = new Vector2[pointsCount * 2];
                foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width - thickness, height - thickness, rotation, angleStart, angleSize, sampling))
                    vertices[vertexIndex++] = point;

                indices = new ushort[(pointsCount - 1) * 6];

                for (int i = 0; i < pointsCount - 1; i++)
                {
                    int j = i * 6;
                    indices[j] = (ushort)i;
                    indices[j + 1] = (ushort)(pointsCount + i);
                    indices[j + 2] = (ushort)(pointsCount + i + 1);
                    indices[j + 3] = (ushort)i;
                    indices[j + 4] = (ushort)(pointsCount + i + 1);
                    indices[j + 5] = (ushort)(i + 1);
                }

                if (completed)
                {
                    int j = (pointsCount - 2) * 6;
                    indices[j + 2] = (ushort)pointsCount;
                    indices[j + 4] = (ushort)pointsCount;
                    indices[j + 5] = 0;
                }
            }
            else
            {
                vertices = new Vector2[pointsCount + 1];
                vertices[vertexIndex++] = center;

                indices = new ushort[pointsCount * 3];

                for (int i = 0; i < pointsCount; i++)
                {
                    int j = i * 3;
                    indices[j] = 0;
                    indices[j + 1] = (ushort)(i + 1);
                    indices[j + 2] = (ushort)(i + 2);
                }

                if (completed)
                    indices[(pointsCount - 1) * 3 + 2] = 1;
            }

            foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width, height, rotation, angleStart, angleSize, sampling))
                vertices[vertexIndex++] = point;
            
            Vertices = new ReadOnlyCollection<Vector2>(vertices);
            Indices = new ReadOnlyCollection<ushort>(indices);
            VertexCount = vertices.Length;
            IndexCount = indices.Length;
        }

        public EllipsePrimitive(Color color, Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, thickness, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color, color };
        }

        public EllipsePrimitive(Color centerColor, Color borderColor, Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, thickness, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { centerColor, borderColor };
        }

        protected override Color GetVertexColor(int i)
        {
            if (_hasInnerSize)
                return i < VertexCount / 2 ? Colors[0] : Colors[1];
            
            return i == 0 ? Colors[0] : Colors[1];
        }
    }
}