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
        
        public override IReadOnlyCollection<Vector2> Vertices { get; }
        public override IReadOnlyCollection<ushort> Indices { get; }
        public override sealed Color[] Colors { get; set; }
        private bool _hasInnerSize;

        public EllipsePrimitive(Vector2 center, float width, float height, float thickness = float.MaxValue, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            Vector2[] vertices;
            ushort[] indexes;
            
            int vertexIndex = 0;
            int pointsCount = PrimitiveHelpers.GetEllipseOutlinePointsCount(angleSize, sampling, out bool completed);
            _hasInnerSize = thickness < width && thickness < height;

            if (_hasInnerSize)
            {
                vertices = new Vector2[pointsCount * 2];
                foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width - thickness, height - thickness, rotation, angleStart, angleSize, sampling))
                    vertices[vertexIndex++] = point;

                indexes = new ushort[(pointsCount - 1) * 6];

                for (int i = 0; i < pointsCount - 1; i++)
                {
                    int j = i * 6;
                    indexes[j] = (ushort)i;
                    indexes[j + 1] = (ushort)(pointsCount + i);
                    indexes[j + 2] = (ushort)(pointsCount + i + 1);
                    indexes[j + 3] = (ushort)i;
                    indexes[j + 4] = (ushort)(pointsCount + i + 1);
                    indexes[j + 5] = (ushort)(i + 1);
                }

                if (completed)
                {
                    int j = (pointsCount - 2) * 6;
                    indexes[j + 2] = (ushort)pointsCount;
                    indexes[j + 4] = (ushort)pointsCount;
                    indexes[j + 5] = 0;
                }
            }
            else
            {
                vertices = new Vector2[pointsCount + 1];
                vertices[vertexIndex++] = center;

                indexes = new ushort[pointsCount * 3];

                for (int i = 0; i < pointsCount - 1; i++)
                {
                    int j = i * 3;
                    indexes[j] = 0;
                    indexes[j + 1] = (ushort)(i + 1);
                    indexes[j + 2] = (ushort)(i + 2);
                }

                if (completed)
                    indexes[(pointsCount - 2) * 3 + 2] = 1;
            }

            foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width, height, rotation, angleStart, angleSize, sampling))
                vertices[vertexIndex++] = point;
            
            Vertices = new ReadOnlyCollection<Vector2>(vertices);
            Indices = new ReadOnlyCollection<ushort>(indexes);
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
                return i < Vertices.Count / 2 ? Colors[0] : Colors[1];
            
            return i == 0 ? Colors[0] : Colors[1];
        }

        public override void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, verticesIndex, indicesIndex, Indices.Count / 3);
        }
    }
}