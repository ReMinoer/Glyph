using System.Collections.Generic;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Core.Colliders
{
    static public class CollisionUtils
    {
        static public bool IsColliding<TShape, TOther>(CollisionDelegate<TShape, TOther> collisionDelegate, ICollider<TShape> shape, ICollider<TOther> other, out Collision collision)
            where TOther : IShape
            where TShape : IShape
        {
            if (collisionDelegate(shape.Shape, other.Shape, out Vector2 correction))
            {
                collision = new Collision
                {
                    Sender = shape,
                    OtherCollider = other,
                    Correction = correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }
        
        static public bool IsShapeCollidingGrid<TShape>(CollisionDelegate<TShape, TopLeftRectangle> rectangleCollisionDelegate,
            ICollider<TShape> shapeCollider, IGridCollider gridCollider, out Collision collision)
            where TShape : IShape
        {
            IEnumerable<int[]> shapeGridBox = gridCollider.Grid.IndexIntersection(
                (x, y) => rectangleCollisionDelegate(y, x, out _),
                shapeCollider.Shape,
                (i, j) => gridCollider.IsCollidableCase(shapeCollider, i, j));

            foreach (int[] indexes in shapeGridBox)
            {
                int i = indexes[0];
                int j = indexes[1];

                var rectangle = new TopLeftRectangle(gridCollider.Grid.ToWorldPoint(i, j), gridCollider.Grid.Delta);

                bool colliding = false;
                for (int x = -1; x <= 1 && !colliding; x++)
                    for (int y = -1; y <= 1 && !colliding; y++)
                    {
                        if ((x + y) % 2 == 0)
                            continue;

                        if (!gridCollider.Grid.ContainsPoint(i + y, j + x) || !gridCollider.IsCollidableCase(shapeCollider, i + y, j + x))
                            continue;

                        var otherCase = new TopLeftRectangle(gridCollider.Grid.ToWorldPoint(i + y, j + x), gridCollider.Grid.Delta);

                        if (!rectangleCollisionDelegate(shapeCollider.Shape, otherCase, out _))
                            continue;

                        rectangle = new TopLeftRectangle
                        {
                            Position = (gridCollider.Grid.ToWorldPoint(i, j) + gridCollider.Grid.ToWorldPoint(i + y, j + x)) * 0.5f,
                            Size = gridCollider.Grid.Delta + gridCollider.Grid.Delta.Multiply(System.Math.Abs(x), System.Math.Abs(y))
                        };

                        colliding = true;
                    }

                if (!rectangleCollisionDelegate(shapeCollider.Shape, rectangle, out Vector2 correction))
                    continue;

                collision = new Collision
                {
                    Sender = shapeCollider,
                    OtherCollider = gridCollider,
                    Correction = correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }
    }
}