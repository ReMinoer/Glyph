using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteRenderer : GlyphComponent, IDraw
    {
        [Injectable]
        public SpriteDescriptor Sprite { get; set; }

        public bool Visible { get; set; }
        private readonly SceneNode _sceneNode;
        private readonly SpriteBatch _spriteBatch;

        public SpriteRenderer(SceneNode sceneNode, SpriteBatch spriteBatch)
        {
            _sceneNode = sceneNode;
            _spriteBatch = spriteBatch;
        }

        public void Draw()
        {
            if (!Visible || Sprite == null)
                return;

            _spriteBatch.Draw(Sprite.Texture, _sceneNode.Position, Sprite.SourceRectangle, Sprite.Color,
                _sceneNode.Rotation, Sprite.Origin, _sceneNode.Scale * Sprite.Scale, Sprite.Effects, 0);
        }
    }
}