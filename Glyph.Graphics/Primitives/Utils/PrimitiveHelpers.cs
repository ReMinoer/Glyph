using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics.Primitives
{
    static public class PrimitiveHelpers
    {
        static public int GetEllipseOutlinePointsCount(float angleSize, int sampling, out bool completed, out bool additionalPoint)
        {
            double count = sampling * angleSize / MathHelper.TwoPi;
            
            additionalPoint = count > System.Math.Floor(count);
            completed = angleSize >= MathHelper.TwoPi;
            
            if (additionalPoint || !completed)
                count++;
            
            return (int)System.Math.Ceiling(count);
        }

        static public IEnumerable<Vector2> GetEllipseOutlinePoints(Vector2 center, float width, float height, float rotation, float angleStart, float angleSize, int sampling)
        {
            rotation = MathHelper.WrapAngle(rotation);
            angleStart = MathHelper.WrapAngle(angleStart);

            Matrix? rotationMatrix = null;
            if (!rotation.EqualsZero())
                rotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Backward, rotation);
            
            int stepCount = GetEllipseOutlinePointsCount(angleSize, sampling, out bool completed, out _);
            float stepSize = angleSize / (!completed ? stepCount - 1 : stepCount);
            
            for (int i = 0; i < stepCount; i++)
            {
                float step = angleStart + i * stepSize;
                Vector2 vertex = center + new Vector2((float)System.Math.Cos(step) * width, (float)System.Math.Sin(step) * height);
                
                if (rotationMatrix.HasValue)
                    vertex = Vector2.Transform(vertex, rotationMatrix.Value);

                yield return vertex;
            }
        }
    }
}