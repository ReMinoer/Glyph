using Glyph.Animation;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class FillingRenderer : Renderer
    {
        private readonly IRectangle _rectangle;

        public FillingRenderer(IRectangle rectangle, ISpriteSource source, SceneNode sceneNode)
            : base(source, sceneNode)
        {
            _rectangle = rectangle;
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            if (SpriteTransformer != null)
                spriteBatch.Draw(Source.Texture, _rectangle.ToStruct(), Source.Rectangle, SpriteTransformer.Color,
                    SceneNode.Rotation, SpriteTransformer.Origin, SpriteTransformer.Effects, 0);
            else
                spriteBatch.Draw(Source.Texture, _rectangle.ToStruct(), Source.Rectangle, Color.White,
                    SceneNode.Rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}