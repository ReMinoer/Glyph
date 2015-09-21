using Glyph.Animation;
using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Physics.Colliders
{
    public abstract class ShapeColliderBase<TShape> : ColliderBase, ICollider<TShape>
        where TShape : IShape
    {
        protected readonly ShapedSpriteBase ShapedSprite;
        protected readonly SpriteTransformer SpriteTransformer;
        private readonly SpriteRenderer _spriteRenderer;
        public TShape Shape { get; protected set; }

        protected ShapeColliderBase(ShapedSpriteBase shapedSprite, SceneNode sceneNode, SpriteBatch spriteBatch)
        {
            ShapedSprite = shapedSprite;
            SpriteTransformer = new SpriteTransformer(ShapedSprite);
            _spriteRenderer = new SpriteRenderer(ShapedSprite, SpriteTransformer, sceneNode, spriteBatch);
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            ShapedSprite.LoadContent(contentLibrary);
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