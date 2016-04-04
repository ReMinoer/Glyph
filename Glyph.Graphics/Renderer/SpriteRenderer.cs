using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class SpriteRenderer : RendererBase
    {
        private readonly SceneNode _sceneNode;

        public SpriteRenderer(ISpriteSource source, SceneNode sceneNode)
            : base(source)
        {
            _sceneNode = sceneNode;
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            if (SpriteTransformer != null)
                spriteBatch.Draw(Source.Texture, _sceneNode.Position, Source.Rectangle, SpriteTransformer.Color,
                    _sceneNode.Rotation, SpriteTransformer.Origin, _sceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, _sceneNode.Depth);
            else
            {
                spriteBatch.Draw(Source.Texture, _sceneNode.Position, Source.Rectangle, Color.White,
                    _sceneNode.Rotation, Source.GetDrawnRectangle().Center.ToVector2(), _sceneNode.Scale, SpriteEffects.None, _sceneNode.Depth);
            }
        }
    }
}