using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class FillingRenderer : RendererBase
    {
        private readonly FillingRectangle _fillingRectangle;
        private readonly SceneNode _sceneNode;

        public FillingRenderer(FillingRectangle fillingRectangle, ISpriteSource source, SceneNode sceneNode)
            : base(source)
        {
            _fillingRectangle = fillingRectangle;
            _sceneNode = sceneNode;
        }

        protected override void Render(IDrawer drawer)
        {
            if (SpriteTransformer != null)
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _fillingRectangle.Rectangle.ToStruct(), Source.Rectangle,
                    SpriteTransformer.Color, _sceneNode.Rotation, SpriteTransformer.Origin, SpriteTransformer.Effects, _sceneNode.Depth);
            else
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _fillingRectangle.Rectangle.ToStruct(), Source.Rectangle,
                    Color.White, _sceneNode.Rotation, Vector2.Zero, SpriteEffects.None, _sceneNode.Depth);
        }
    }
}