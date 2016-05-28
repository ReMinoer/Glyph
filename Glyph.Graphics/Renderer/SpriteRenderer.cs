using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class SpriteRenderer : RendererBase
    {
        private readonly SceneNode _sceneNode;

        protected override float DepthProtected
        {
            get { return _sceneNode.Depth; }
        }

        public SpriteRenderer(ISpriteSource source, SceneNode sceneNode)
            : base(source)
        {
            _sceneNode = sceneNode;
        }

        protected override void Render(IDrawer drawer)
        {
            if (SpriteTransformer != null)
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _sceneNode.Position, Source.Rectangle, SpriteTransformer.Color,
                    _sceneNode.Rotation, SpriteTransformer.Origin, _sceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, _sceneNode.Depth);
            else
            {
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _sceneNode.Position, Source.Rectangle, Color.White,
                    _sceneNode.Rotation, Source.GetDefaultOrigin(), _sceneNode.Scale, SpriteEffects.None, _sceneNode.Depth);
            }
        }
    }
}