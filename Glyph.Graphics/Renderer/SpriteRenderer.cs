using Glyph.Composition;
using Glyph.Core;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class SpriteRenderer : RendererBase
    {
        private readonly SceneNode _sceneNode;
        protected override ISceneNode SceneNode => _sceneNode;

        protected override float DepthProtected
        {
            get { return _sceneNode.Depth; }
        }

        public override IArea Area
        {
            get
            {
                if (Source == null)
                    return null;

                Rectangle rectangle = Source.GetDrawnRectangle();
                Vector2 origin = SpriteTransformer?.Origin ?? Source.GetDefaultOrigin();
                Vector2 scale = _sceneNode.Scale * (SpriteTransformer?.Scale ?? Vector2.One);

                return new TopLeftRectangle(_sceneNode.Position - origin * scale, rectangle.Size.ToVector2() * scale);
            }
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