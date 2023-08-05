using System;
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
        protected override ISceneNode SceneNode => _fillingRectangle.SceneNode;
        
        public Quad Shape => _fillingRectangle.Rectangle;
        public override IArea Area => _fillingRectangle.Rectangle;

        public FillingRenderer(FillingRectangle fillingRectangle, ISpriteSource source)
            : base(source)
        {
            _fillingRectangle = fillingRectangle;
            SubscribeDepthChanged(_fillingRectangle.SceneNode);
        }

        protected override void Render(IDrawer drawer)
        {
            if (SpriteTransformer != null)
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _fillingRectangle.Rectangle.P0, Source.Rectangle, SpriteTransformer.Color,
                    SceneNode.Rotation, SpriteTransformer.Origin, _fillingRectangle.Rectangle.Size / Source.Texture.Size(), SpriteTransformer.Effects, 0);
            else
                drawer.SpriteBatchStack.Current.Draw(Source.Texture, _fillingRectangle.Rectangle.P0, Source.Rectangle, Color.White,
                    SceneNode.Rotation, Vector2.Zero, _fillingRectangle.Rectangle.Size / Source.Texture.Size(), SpriteEffects.None, 0);
        }
    }
}