using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Primitives
{
    public class EllipseOutlinePrimitive : IPrimitive
    {
        public const int DefaultSampling = 64;
        public Color[] Colors { get; set; }

        private readonly IReadOnlyList<Vector2> _readOnlyVertices;
        IReadOnlyCollection<Vector2> IPrimitive.Vertices => _readOnlyVertices;

        public EllipseOutlinePrimitive(Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
        {
            rotation = MathHelper.WrapAngle(rotation);
            angleStart = MathHelper.WrapAngle(angleStart);

            Matrix? rotationMatrix = null;
            if (!rotation.EqualsZero())
                rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Backward, rotation);
            
            int stepCount = (int)System.Math.Ceiling(sampling * angleSize / MathHelper.TwoPi);
            float stepSize = angleSize / stepCount;

            var vertices = new Vector2[stepCount + 1];
            for (int i = 0; i < stepCount + 1; i++)
            {
                float step = angleStart + i * stepSize;
                vertices[i] = center + new Vector2((float)System.Math.Cos(step) * width, (float)System.Math.Sin(step) * height);
                
                if (rotationMatrix.HasValue)
                    vertices[i] = Vector2.Transform(vertices[i], rotationMatrix.Value);
            }
            
            _readOnlyVertices = new ReadOnlyCollection<Vector2>(vertices);
        }

        public EllipseOutlinePrimitive(Color color, Vector2 center, float width, float height, float rotation = 0, float angleStart = 0, float angleSize = MathHelper.TwoPi, int sampling = DefaultSampling)
            : this(center, width, height, rotation, angleStart, angleSize, sampling)
        {
            Colors = new[] { color };
        }

        public void CopyToArray(VertexPositionColor[] vertexArray, int startIndex)
        {
            for (int i = 0; i < _readOnlyVertices.Count; i++)
                vertexArray[startIndex + i] = new VertexPositionColor(_readOnlyVertices[i].ToVector3(), Colors[i % Colors.Length]);
        }

        public void DrawPrimitives(GraphicsDevice graphicsDevice, int startIndex)
        {
            graphicsDevice.DrawPrimitives(PrimitiveType.LineStrip, startIndex, _readOnlyVertices.Count - 1);
        }
    }
}