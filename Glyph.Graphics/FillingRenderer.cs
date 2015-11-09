using Glyph.Animation;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class FillingRenderer : Renderer
    {
        private readonly IBounded _bounded;

        public FillingRenderer(IBounded bounded, ISpriteSource source, SceneNode sceneNode)
            : base(source, sceneNode)
        {
            _bounded = bounded;
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            if (SpriteTransformer != null)
                spriteBatch.Draw(Source.Texture, _bounded.Bounds.ToStruct(), Source.Rectangle, SpriteTransformer.Color,
                    SceneNode.Rotation, SpriteTransformer.Origin, SpriteTransformer.Effects, 0);
            else
                spriteBatch.Draw(Source.Texture, _bounded.Bounds.ToStruct(), Source.Rectangle, Color.White,
                    SceneNode.Rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}