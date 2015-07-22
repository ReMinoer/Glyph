using Glyph.Animation;
using Glyph.Graphics;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public abstract class ShapeColliderBase<TShape> : ColliderBase, ICollider<TShape>
        where TShape : IShape
    {
        protected readonly SpriteDescriptor SpriteDescriptor;
        private readonly SpriteRenderer _spriteRenderer;
        public TShape Shape { get; protected set; }

        protected ShapeColliderBase(SceneNode sceneNode, SpriteBatch spriteBatch)
        {
            SpriteDescriptor = new SpriteDescriptor();
            _spriteRenderer = new SpriteRenderer(sceneNode, spriteBatch)
            {
                Sprite = SpriteDescriptor
            };
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return Shape.Contains(point);
        }

        public override void Draw()
        {
            if (!Visible)
                return;

            _spriteRenderer.Draw();
        }

        public override void Dispose()
        {
            SpriteDescriptor.Dispose();
            base.Dispose();
        }
    }
}