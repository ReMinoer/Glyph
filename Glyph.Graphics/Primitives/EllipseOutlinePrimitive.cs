using System.Collections.Generic;
using System.Collections.ObjectModel;
using Glyph.Graphics.Primitives.Base;
using Glyph.Graphics.Primitives.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EllipseOutlinePrimitive : PrimitiveBase
    {
        public const int DefaultSampling = 64;

        public override PrimitiveType PrimitiveType => PrimitiveType.LineStrip;
        
        public override IEnumerable<Vector2> Vertices { get; }
        public override IEnumerable<ushort> Indices => null;
        public override int VertexCount { get; }
        public override int IndexCount => 0;
        public override sealed Color[] Colors { get; set; }

        public EllipseOutlinePrimitive(Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            int i = 0;
            int outlinePointsCount = PrimitiveHelpers.GetEllipseOutlinePointsCount(angleSize, sampling, out bool completed);

            if (completed)
                outlinePointsCount++;

            var vertices = new Vector2[outlinePointsCount];
            foreach (Vector2 point in PrimitiveHelpers.GetEllipseOutlinePoints(center, width, height, rotation, angleStart, angleSize, sampling))
                vertices[i++] = point;
            if (completed)
                vertices[i] = vertices[0];

            Vertices = new ReadOnlyCollection<Vector2>(vertices);
            VertexCount = vertices.Length;
        }

        public EllipseOutlinePrimitive(Color color, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }
    }
}