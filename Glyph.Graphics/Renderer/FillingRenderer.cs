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
            var drawnRectangle = new CenteredRectangle
            {
                Center = _sceneNode.Position + _fillingRectangle.Rectangle.Center,
                Size = _fillingRectangle.Rectangle.Size
            };

            if (SpriteTransformer != null)
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, drawnRectangle.ToIntegers(), Source.Rectangle,
                    SpriteTransformer.Color, _sceneNode.Rotation, SpriteTransformer.Origin, SpriteTransformer.Effects, _sceneNode.Depth);
            else
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, drawnRectangle.ToIntegers(), Source.Rectangle,
                    Color.White, _sceneNode.Rotation, Vector2.Zero, SpriteEffects.None, _sceneNode.Depth);
        }
    }
}