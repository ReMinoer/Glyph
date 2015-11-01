using System;
using Diese.Injection;
using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteRenderer : GlyphComponent, IDraw
    {
        private readonly ISpriteSource _source;
        private readonly SceneNode _sceneNode;
        private readonly Lazy<SpriteBatch> _lazySpriteBatch;
        public bool Visible { get; set; }

        [Injectable]
        public SpriteTransformer SpriteTransformer { get; set; }

        public SpriteRenderer(ISpriteSource source, SceneNode sceneNode, Lazy<SpriteBatch> lazySpriteBatch)
        {
            _source = source;
            _sceneNode = sceneNode;
            _lazySpriteBatch = lazySpriteBatch;

            Visible = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || _source.Texture == null)
                return;

            if (SpriteTransformer != null)
                _lazySpriteBatch.Value.Draw(_source.Texture, _sceneNode.Position, SpriteTransformer.SourceRectangle, SpriteTransformer.Color,
                    _sceneNode.Rotation, SpriteTransformer.Origin, _sceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, 0);
            else
                _lazySpriteBatch.Value.Draw(_source.Texture, _sceneNode.Position, null, Color.White,
                    _sceneNode.Rotation, Vector2.Zero, _sceneNode.Scale, SpriteEffects.None, 0);
        }
    }
}