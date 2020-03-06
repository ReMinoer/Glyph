using Glyph.Composition;
using Glyph.Core;
using Glyph.Graphics.Renderer.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class FillingRenderer : SpriteRendererBase
    {
        private readonly FillingRectangle _fillingRectangle;
        private readonly SceneNode _sceneNode;
        protected override ISceneNode SceneNode => _sceneNode;

        protected override float DepthProtected => _sceneNode.Depth;

        public Quad Shape => _sceneNode.Transformation.Transform(new CenteredRectangle(Vector2.Zero, _fillingRectangle.Rectangle.Size));
        public override IArea Area => new TopLeftRectangle
        {
            Position = _sceneNode.Position + _fillingRectangle.Rectangle.Position,
            Size = _fillingRectangle.Rectangle.Size
        };

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