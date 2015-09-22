using System;
using Glyph.Animation;
using Glyph.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public class SpriteRenderer : GlyphComponent, IDraw
    {
        private readonly ISpriteSource _source;
        private readonly SpriteTransformer _transformer;
        private readonly SceneNode _sceneNode;
        private readonly Lazy<SpriteBatch> _lazySpriteBatch;
        public bool Visible { get; set; }

        public SpriteRenderer(ISpriteSource source, SpriteTransformer transformer, SceneNode sceneNode, Lazy<SpriteBatch> lazySpriteBatch)
        {
            _source = source;
            _transformer = transformer;
            _sceneNode = sceneNode;
            _lazySpriteBatch = lazySpriteBatch;
        }

        public void Draw()
        {
            if (!Visible || _source.Texture == null)
                return;

            _lazySpriteBatch.Value.Draw(_source.Texture, _sceneNode.Position, _transformer.SourceRectangle, _transformer.Color,
                _sceneNode.Rotation, _transformer.Origin, _sceneNode.Scale * _transformer.Scale, _transformer.Effects, 0);
        }
    }
}