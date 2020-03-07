using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public EllipsePrimitive(Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            int i = 0;
            int outlinePointsCount = PrimitiveHelpers.GetEllipseOutlinePointsCount(angleSize, sampling, out bool completed, out bool additionalPoint);

            var vertices = new Vector2[outlinePointsCount + 1];
            vertices[i++] = center;
            foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width, height, rotation, angleStart, angleSize, sampling))
                vertices[i++] = point;

            Vertices = new ReadOnlyCollection<Vector2>(vertices);
            
            var indexes = new ushort[outlinePointsCount * 3];
            if (completed)
            {
                for (i = 0; i < outlinePointsCount - 1; i++)
                {
                    indexes[i * 3] = 0;
                    indexes[i * 3 + 1] = (ushort)i;
                    indexes[i * 3 + 2] = (ushort)(i + 1);
                }
                indexes[i * 3] = 0;
                indexes[i * 3 + 1] = (ushort)i;
                indexes[i * 3 + 2] = 1;
            }
            else
            {
                for (i = 0; i < outlinePointsCount; i++)
                {
                    indexes[i * 3] = 0;
                    indexes[i * 3 + 1] = (ushort)i;
                    indexes[i * 3 + 2] = (ushort)(i + 1);
                }
            }
            
            Indices = new ReadOnlyCollection<ushort>(indexes);
        }

        public EllipsePrimitive(Color color, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color, color };
        }

        public EllipsePrimitive(Color centerColor, Color borderColor, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { centerColor, borderColor };
        }

        protected override Color GetVertexColor(int i) => i == 0 ? Colors[0] : Colors[1];

        public override void DrawPrimitives(GraphicsDevice graphicsDevice, int verticesIndex, int indicesIndex)
        {
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, verticesIndex, indicesIndex, Indices.Count / 3);
        }
    }
}