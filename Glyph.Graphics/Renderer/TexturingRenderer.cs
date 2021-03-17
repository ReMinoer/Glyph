using System;
using Glyph.Graphics.Renderer.Base;
using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class TexturingRenderer : SpriteRendererBase
    {
        private readonly FillingRectangle _fillingRectangle;
        protected override ISceneNode SceneNode => _fillingRectangle.SceneNode;

        public override IArea Area => _fillingRectangle.Rectangle;

        public TexturingRenderer(FillingRectangle fillingRectangle, ISpriteSource source)
            : base(source)
        {
            _fillingRectangle = fillingRectangle;
            SubscribeDepthChanged(_fillingRectangle.SceneNode);
        }

        protected override void Render(IDrawer drawer)
        {
            TopLeftRectangle cameraRectangle = drawer.DisplayedRectangle.BoundingBox;
            TopLeftRectangle drawnRectangle = _fillingRectangle.Rectangle.BoundingBox;

            if (!drawnRectangle.Intersects(cameraRectangle, out TopLeftRectangle visibleRectangle))
                return;

            TopLeftRectangle sourceRectangle = Source.GetDrawnRectangle().ToFloats();

            Vector2 diff = cameraRectangle.Position - drawnRectangle.Position;
            Vector2 sourcePatchInit = new Vector2(MathHelper.Max(diff.X, 0), MathHelper.Max(diff.Y, 0));
            Vector2 sourcePatchOrigin = sourcePatchInit;

            float y = 0;
            while (y < visibleRectangle.Height)
            {
                float sourceRemainingHeight = sourceRectangle.Height - sourcePatchOrigin.Y;
                float destinationRemainingHeight = visibleRectangle.Height - y;
                float sourcePatchHeight = MathHelper.Min(sourceRemainingHeight, destinationRemainingHeight);

                sourcePatchOrigin.X = sourcePatchInit.X;

                float x = 0;
                while (x < visibleRectangle.Width)
                {
                    float sourceRemainingWidth = sourceRectangle.Width - sourcePatchOrigin.X;
                    float destinationRemainingWidth = visibleRectangle.Width - x;
                    float sourcePatchWidth = MathHelper.Min(sourceRemainingWidth, destinationRemainingWidth);

                    Vector2 position = visibleRectangle.Position + (SceneNode.Transform(new Vector2(x, y)) - SceneNode.Position);
                    var sourcePatch = new TopLeftRectangle(sourcePatchOrigin, new Vector2(sourcePatchWidth, sourcePatchHeight));

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToIntegers(), SpriteTransformer.Color,
                            SceneNode.Rotation, Vector2.Zero, SceneNode.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, SceneNode.Depth);
                    else
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToIntegers(), Color.White,
                            SceneNode.Rotation, Vector2.Zero, SceneNode.Scale, SpriteEffects.None, SceneNode.Depth);

                    x += sourcePatchWidth;
                    sourcePatchOrigin.X = sourceRectangle.Position.X;
                }

                y += sourcePatchHeight;
                sourcePatchOrigin.Y = sourceRectangle.Position.Y;
            }
        }
    }
}