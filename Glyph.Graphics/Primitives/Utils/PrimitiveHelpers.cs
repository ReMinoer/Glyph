using System.Collections.Generic;
using System.Linq;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives.Utils
{
    static public class PrimitiveHelpers
    {
        static public IEnumerable<Vector2> GetOrthographicTextureCoordinates(IPrimitive primitive)
        {
            TopLeftRectangle boundingBox = MathUtils.GetBoundingBox(primitive.Vertices);
            return primitive.Vertices.Select(x => x.Rescale(boundingBox, new TopLeftRectangle(0, 0, 1, 1)));
        }

        static public int GetEllipseOutlinePointsCount(float angleSize, int sampling)
        {
            double count = sampling * angleSize / MathHelper.TwoPi;
            
            bool additionalPoint = count > System.Math.Floor(count);
            bool completed = angleSize >= MathHelper.TwoPi;
            
            if (additionalPoint || !completed)
                count++;
            
            return (int)System.Math.Ceiling(count);
        }

        static public IEnumerable<Vector2> GetEllipseOutlinePoints(Vector2 center, float width, float height, float rotation, float angleStart, float angleSize, int sampling)
        {
            rotation = MathHelper.WrapAngle(rotation);
            angleStart = MathHelper.WrapAngle(angleStart);

            bool completed = angleSize >= MathHelper.TwoPi;

            Matrix? rotationMatrix = null;
            if (!rotation.EqualsZero())
                rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Backward, rotation);

            int stepCount = GetEllipseOutlinePointsCount(angleSize, sampling);
            float stepSize = angleSize / (!completed ? stepCount - 1 : stepCount);

            for (int i = 0; i < stepCount; i++)
            {
                float step = angleStart + i * stepSize;
                var vertex = new Vector2((float)System.Math.Cos(step) * width, (float)System.Math.Sin(step) * height);

                if (rotationMatrix.HasValue)
                    vertex = Vector2.Transform(vertex, rotationMatrix.Value);

                yield return center + vertex;
            }
        }
    }
}