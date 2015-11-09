using Glyph.Graphics;
using Glyph.Graphics.Shapes;
using Glyph.Math.Shapes;
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

        protected ShapeColliderBase(ShapedSpriteBase shapedSprite, Context context)
            : base(context)
        {
            ShapedSprite = shapedSprite;
            SpriteTransformer = new SpriteTransformer();
            _spriteRenderer = new SpriteRenderer(ShapedSprite, SceneNode);

            SceneNode.Refreshed += x => Shape.Center = x.Position;
        }

        public override void LoadContent(ContentLibrary contentLibrary)
        {
            ShapedSprite.LoadContent(contentLibrary);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            _spriteRenderer.Draw(spriteBatch);
        }

        public override void Dispose()
        {
            SpriteTransformer.Dispose();
            base.Dispose();
        }
    }
}