using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface ISpriteDescriptor
    {
        Texture2D Texture { get; }
        Rectangle? SourceRectangle { get; }
        Color Color { get; }
        Vector2 Origin { get; }
        SpriteEffects Effects { get; }
    }

    public class SpriteRenderer : GlyphComponent, IDraw
    {
        private readonly SceneNode _sceneNode;
        private readonly SpriteBatch _spriteBatch;
        public bool Visible { get; set; }
        public ISpriteDescriptor Sprite { get; set; }

        public SpriteRenderer(SceneNode sceneNode, SpriteBatch spriteBatch)
        {
            _sceneNode = sceneNode;
            _spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            if (!Visible)
                return;

            _spriteBatch.Draw(Sprite.Texture, _sceneNode.Position, Sprite.SourceRectangle, Sprite.Color,
                _sceneNode.Rotation, Sprite.Origin, _sceneNode.Scale, Sprite.Effects, 0);
        }
    }
}