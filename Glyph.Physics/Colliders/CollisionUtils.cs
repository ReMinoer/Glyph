using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Physics.Colliders
{
    static public class CollisionUtils
    {
        static public bool IsColliding<TShape, TOther>(CollisionDelegate<TShape, TOther> collisionDelegate, ICollider<TShape> shape, ICollider<TOther> other, out Collision collision)
            where TOther : IShape where TShape : IShape
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


        // TODO : Handle multi collisions with grid
        static public bool IsShapeCollidingGrid<TShape>(CollisionDelegate<TShape, IRectangle> collisionDelegate, ICollider<TShape> shape, IGridCollider gridCollider, out Collision collision)
            where TShape : IShape
        {
            Point topleft = gridCollider.Grid.ToGridPoint(shape.Center - shape.BoundingBox.Size / 2);
            Point bottomRight = gridCollider.Grid.ToGridPoint(shape.Center + shape.BoundingBox.Size / 2);

            for (int i = topleft.Y; i <= bottomRight.Y; i++)
                for (int j = topleft.X; j <= bottomRight.X; j++)
                {
                    if (!gridCollider.IsCollidableCase(shape, i, j))
                        continue;

                    IRectangle rectangle = new OriginRectangle(gridCollider.Grid.ToWorldPoint(i, j), gridCollider.Grid.Delta);

                    bool colliding = false;
                    for (int x = -1; x <= 1 && !colliding; x++)
                        for (int y = -1; y <= 1 && !colliding; y++)
                        {
                            if ((x + y) % 2 == 0)
                                continue;

                            if (!gridCollider.IsCollidableCase(shape, i + y, j + x))
                                continue;

                            rectangle = new CenteredRectangle
                            {
                                Center = (gridCollider.Grid.ToWorldPoint(i, j) + gridCollider.Grid.ToWorldPoint(i + y, j + x) + gridCollider.Grid.Delta) * 0.5f,
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

        static public bool IsGridCollidingShape<TShape>(CollisionDelegate<IRectangle, TShape> collisionDelegate, IGridCollider gridCollider, ICollider<TShape> shape, out Collision collision)
            where TShape : IShape
        {
            Point topleft = gridCollider.Grid.ToGridPoint(shape.Center - shape.BoundingBox.Size / 2);
            Point bottomRight = gridCollider.Grid.ToGridPoint(shape.Center + shape.BoundingBox.Size / 2);

            for (int i = topleft.Y; i <= bottomRight.Y; i++)
                for (int j = topleft.X; j <= bottomRight.X; j++)
                {
                    if (!gridCollider.IsCollidableCase(shape, i, j))
                        continue;

                    IRectangle rectangle = new OriginRectangle(gridCollider.Grid.ToWorldPoint(i, j), gridCollider.Grid.Delta);

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