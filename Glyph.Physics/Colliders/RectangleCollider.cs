using System;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class RectangleCollider : ShapeColliderBase<CenteredRectangle>
    {
        private float _width;
        private float _height;

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RefreshScale();
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RefreshScale();
            }
        }

        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
            set
            {
                _width = value.X;
                _height = value.Y;
                RefreshScale();
            }
        }

        public override CenteredRectangle Bounds
        {
            get { return new CenteredRectangle(Center, Width, Height); }
        }

        public RectangleCollider(Context context, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(new FilledRectangleSprite(lazyGraphicsDevice), context)
        {
            Size = new Vector2(100, 100);
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            IRectangle intersection;
            if (IntersectionUtils.TwoRectangles(Bounds, collider.Bounds, out intersection))
            {
                Vector2 correction;

                bool isWiderThanTall = intersection.Width > intersection.Height;

                if (isWiderThanTall)
                {
                    correction = Bounds.Top <= collider.Bounds.Top
                        ? new Vector2(0, -intersection.Height)
                        : new Vector2(0, intersection.Height);
                }
                else
                {
                    correction = Bounds.Right >= collider.Bounds.Right
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
            if (IntersectionUtils.RectangleAndCircle(Bounds, (collider as ICollider<Circle>).Bounds, out correction))
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

        public override bool Intersects(IRectangle rectangle)
        {
            return Bounds.Intersects(rectangle);
        }

        public override bool Intersects(ICircle circle)
        {
            return Bounds.Intersects(circle);
        }

        private void RefreshScale()
        {
            SpriteTransformer.Scale = Size / 100;
        }
    }
}