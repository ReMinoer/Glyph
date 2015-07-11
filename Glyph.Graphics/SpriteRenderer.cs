using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteRenderer : GlyphComponent, IDraw
    {
        private readonly SceneNode _sceneNode;
        private readonly SpriteDescriptor _sprite;
        private readonly SpriteBatch _spriteBatch;
        public bool Visible { get; set; }

        public SpriteRenderer(SceneNode sceneNode, SpriteDescriptor sprite, SpriteBatch spriteBatch)
        {
            _sceneNode = sceneNode;
            _sprite = sprite;
            _spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            if (!Visible)
                return;

            _spriteBatch.Draw(_sprite.Texture, _sceneNode.Position, _sprite.SourceRectangle, _sprite.Color,
                _sceneNode.Rotation, _sprite.Origin, _sceneNode.Scale, _sprite.Effects, 0);
        }
    }
}