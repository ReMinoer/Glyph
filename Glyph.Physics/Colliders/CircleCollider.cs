using System;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public class CircleCollider : ShapeColliderBase<Circle>
    {
        private float _radius;

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                RefreshScale();
            }
        }

        public override Circle Bounds
        {
            get { return new Circle(Center, Radius); }
        }

        public CircleCollider(Context context, Lazy<GraphicsDevice> lazyGraphicsDevice)
            : base(new CircleSprite(lazyGraphicsDevice), context)
        {
            Radius = 100;
        }

        public override bool IsColliding(RectangleCollider collider, out Collision collision)
        {
            Vector2 correction;
            if (IntersectionUtils.RectangleAndCircle((collider as ICollider<CenteredRectangle>).Bounds, Bounds, out correction))
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

        public override bool IsColliding(CircleCollider collider, out Collision collision)
        {
            float radiusIntersection;
            if (IntersectionUtils.TwoCircles(Bounds, collider.Bounds, out radiusIntersection))
            {
                Vector2 direction = (Bounds.Center - collider.Bounds.Center).Normalized();
                Vector2 correction = radiusIntersection * direction;

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
            SpriteTransformer.Scale = new Vector2(Radius) / 100;
        }
    }
}