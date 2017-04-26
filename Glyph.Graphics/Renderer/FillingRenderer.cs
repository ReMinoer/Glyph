using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class FillingRenderer : RendererBase
    {
        private readonly FillingRectangle _fillingRectangle;
        private readonly SceneNode _sceneNode;

        protected override float DepthProtected
        {
            get { return _sceneNode.Depth; }
        }

        public FillingRenderer(FillingRectangle fillingRectangle, ISpriteSource source, SceneNode sceneNode)
            : base(source)
        {
            _fillingRectangle = fillingRectangle;
            _sceneNode = sceneNode;
        }

        protected override void Render(IDrawer drawer)
        {
            if (SpriteTransformer != null)
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _sceneNode.Position + _fillingRectangle.Rectangle.Position, Source.Rectangle,
                    SpriteTransformer.Color, _sceneNode.Rotation, SpriteTransformer.Origin, _fillingRectangle.Rectangle.Size / Source.Texture.Size(), SpriteTransformer.Effects, _sceneNode.Depth);
            else
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _sceneNode.Position + _fillingRectangle.Rectangle.Position, Source.Rectangle,
                    Color.White, _sceneNode.Rotation, Vector2.Zero, _fillingRectangle.Rectangle.Size / Source.Texture.Size(), SpriteEffects.None, _sceneNode.Depth);
        }
    }
}