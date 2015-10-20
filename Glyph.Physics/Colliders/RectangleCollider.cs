using System;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<CenteredRectangle>
    {
        public RectangleCollider(Context context, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(new FilledRectangleSprite(lazyGraphicsDevice), context)
        {
            Shape = new CenteredRectangle(Vector2.Zero, 10, 10);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            base.LoadContent(contentLibrary);
            SpriteTransformer.Scale = Shape.Size;
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            IRectangle intersection;
            if (IntersectionUtils.TwoRectangles(Shape, collider.Shape, out intersection))
            {
                Vector2 correction;

                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = Shape.Top <= collider.Shape.Top
                        ? new Vector2(0, -intersection.Height)
                        : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = Shape.Right >= collider.Shape.Right
                        ? new Vector2(intersection.Width, 0)
                        : new Vector2(-intersection.Width, 0);
                }

                collision = new Collision
                {
                    Sender = this,
                    OtherCollider = collider,
                    Correction = correction,
                    NewPosition = SceneNode.Position + correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            Vector2 correction;
            if (IntersectionUtils.RectangleAndCircle(Shape, collider.Shape, out correction))
            {
                collision = new Collision
                {
                    Sender = this,
                    OtherCollider = collider,
                    Correction = correction,
                    NewPosition = SceneNode.Position + correction
                };

                return true;
            }

            collision = new Collision();
            return false;
        }
    }
}