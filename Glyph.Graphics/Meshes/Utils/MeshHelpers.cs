using System;
using System.Collections.Generic;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Graphics.Meshes.Utils
{
    static public class MeshHelpers
    {
        static public int GetEllipseOutlinePointsCount(float angleSize, int sampling)
        {
            double count = sampling * angleSize / MathHelper.TwoPi;
            
            bool additionalPoint = count > System.Math.Floor(count);
            bool completed = angleSize >= MathHelper.TwoPi;
            
            if (additionalPoint || !completed)
                count++;
            
            return (int)System.Math.Ceiling(count);
        }

        static public IEnumerable<Vector2> GetCircleOutlinePoints(Vector2 center, float radius, int sampling)
            => GetCircleOutlinePoints(center, radius, 0, MathHelper.TwoPi, sampling);

        static public IEnumerable<Vector2> GetCircleOutlinePoints(Vector2 center, float radius, float angleStart, float angleSize, int sampling)
            => GetEllipseOutlinePoints(center, radius, radius, 0, angleStart, angleSize, sampling);

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

        static public List<Rectangle> GetRectanglesFromArray<T>(ITwoDimensionArray<T> array, Func<int, int, T, bool> includingPredicate)
        {
            List<Rectangle> result = new List<Rectangle>();

            // Create boolean mask
            bool[,] mask = new bool[array.GetLength(0), array.GetLength(1)];
            int[] indexes = mask.GetResetIndex();
            while (mask.MoveIndex(indexes))
                mask[indexes[0], indexes[1]] = includingPredicate(indexes[0], indexes[1], array[indexes]);

            mask.GetResetIndex(indexes);
            while (mask.MoveIndex(indexes))
            {
                if (!mask[indexes[0], indexes[1]])
                    continue;

                // Get width on current row
                int width = 1;
                int[] widthIndexes = { indexes[0], indexes[1] };
                if (mask.MoveIndex(widthIndexes, 1))
                {
                    while (mask[widthIndexes[0], widthIndexes[1]])
                    {
                        mask[widthIndexes[0], widthIndexes[1]] = false;
                        width++;
                        if (!mask.MoveIndex(widthIndexes, 1))
                            break;
                    }
                }

                // Add to height all rows matching width
                int height = 1;
                int[] rectangleIndexes = { indexes[0], indexes[1] };
                while (mask.MoveIndex(rectangleIndexes, 0))
                {
                    bool matchingFirstRow = true;
                    for (rectangleIndexes[1] = indexes[1]; rectangleIndexes[1] < indexes[1] + width; rectangleIndexes[1]++)
                    {
                        if (!mask[rectangleIndexes[0], rectangleIndexes[1]])
                        {
                            matchingFirstRow = false;
                            break;
                        }

                        mask[rectangleIndexes[0], rectangleIndexes[1]] = false;
                    }

                    if (!matchingFirstRow)
                    {
                        // Fix mask and break
                        int stopFixIndex = rectangleIndexes[1];
                        for (int k = indexes[1]; k < stopFixIndex; k++)
                        {
                            mask[rectangleIndexes[0], k] = true;
                        }
                        break;
                    }
                    
                    height++;
                }

                result.Add(new Rectangle(indexes[1], indexes[0], width, height));
            }

            return result;
        }
    }
}