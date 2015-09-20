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
        protected readonly SpriteTransformer SpriteTransformer;
        private readonly SpriteRenderer _spriteRenderer;
        public TShape Shape { get; protected set; }

        protected ShapeColliderBase(SceneNode sceneNode, SpriteBatch spriteBatch)
        {
            SpriteTransformer = new SpriteTransformer();
            _spriteRenderer = new SpriteRenderer(SpriteTransformer, sceneNode, spriteBatch);
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
            SpriteTransformer.Dispose();
            base.Dispose();
        }
    }
}