using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Space;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Colliders
{
    static public class CollisionUtils
    {
        static public bool IsColliding<TShape, TOther>(CollisionDelegate<TShape, TOther> collisionDelegate, ICollider<TShape> shape, ICollider<TOther> other, out Collision collision)
            where TOther : IShape
            where TShape : IShape
        {
            Vector2 correction;
            if (collisionDelegate(shape.Shape, other.Shape, out correction))
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
        
        static public bool IsShapeCollidingGrid<TShape>(CollisionDelegate<TShape, TopLeftRectangle> collisionDelegate, ICollider<TShape> shape, IGridCollider gridCollider, out Collision collision)
            where TShape : IShape
        {
            Rectangle gridRange = gridCollider.Grid.ToGridRange(shape.BoundingBox).ClampToRectangle(gridCollider.Grid.IndexesBounds());
            
            for (int i = gridRange.Top; i <= gridRange.Bottom; i++)
                for (int j = gridRange.Left; j <= gridRange.Right; j++)
                {
                    if (!gridCollider.IsCollidableCase(shape, i, j))
                        continue;

                    var rectangle = new TopLeftRectangle(gridCollider.Grid.ToWorldPoint(i, j), gridCollider.Grid.Delta);

                    bool colliding = false;
                    for (int x = -1; x <= 1 && !colliding; x++)
                        for (int y = -1; y <= 1 && !colliding; y++)
                        {
                            if ((x + y) % 2 == 0)
                                continue;

                            if (!gridCollider.Grid.ContainsPoint(i + y, j + x) || !gridCollider.IsCollidableCase(shape, i + y, j + x))
                                continue;

                            var otherCase = new TopLeftRectangle(gridCollider.Grid.ToWorldPoint(i + y, j + x), gridCollider.Grid.Delta);

                            if (!collisionDelegate(shape.Shape, otherCase, out _))
                                continue;

                            rectangle = new TopLeftRectangle
                            {
                                Position = (gridCollider.Grid.ToWorldPoint(i, j) + gridCollider.Grid.ToWorldPoint(i + y, j + x)) * 0.5f,
                                Size = gridCollider.Grid.Delta + gridCollider.Grid.Delta.Multiply(System.Math.Abs(x), System.Math.Abs(y))
                            };

                            colliding = true;
                        }
                    
                    Vector2 correction;
                    if (collisionDelegate(shape.Shape, rectangle, out correction))
                    {
                        collision = new Collision
                        {
                            Sender = shape,
                            OtherCollider = gridCollider,
                            Correction = correction
                        };

                        return true;
                    }
                }

            collision = new Collision();
            return false;
        }

        static public bool IsGridCollidingShape<TShape>(CollisionDelegate<TopLeftRectangle, TShape> collisionDelegate, IGridCollider gridCollider, ICollider<TShape> shape, out Collision collision)
            where TShape : IShape
        {
            Rectangle gridRange = gridCollider.Grid.ToGridRange(shape.BoundingBox).ClampToRectangle(gridCollider.Grid.IndexesBounds());

            for (int i = gridRange.Top; i <= gridRange.Bottom; i++)
                for (int j = gridRange.Left; j <= gridRange.Right; j++)
                {
                    if (!gridCollider.IsCollidableCase(shape, i, j))
                        continue;

                    var rectangle = new TopLeftRectangle(gridCollider.Grid.ToWorldPoint(i, j), gridCollider.Grid.Delta);

                    Vector2 correction;
                    if (collisionDelegate(rectangle, shape.Shape, out correction))
                    {
                        collision = new Collision
                        {
                            Sender = gridCollider,
                            OtherCollider = shape,
                            Correction = correction
                        };

                        return true;
                    }
                }

            collision = new Collision();
            return false;
        }
    }
}