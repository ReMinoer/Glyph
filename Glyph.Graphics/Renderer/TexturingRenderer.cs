using Glyph.Math;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class TexturingRenderer : RendererBase
    {
        private readonly FillingRectangle _fillingRectangle;
        public float Depth { get; set; }

        public TexturingRenderer(FillingRectangle fillingRectangle, ISpriteSource source)
            : base(source)
        {
            _fillingRectangle = fillingRectangle;
        }

        protected override void Render(IDrawer drawer)
        {
            IRectangle drawnRectangle = _fillingRectangle.Rectangle;
            IRectangle cameraRectangle = Camera.DisplayRectangle.ToCenteredRectangle();

            IRectangle visibleRectangle;
            if (!drawnRectangle.Intersects(cameraRectangle, out visibleRectangle))
                return;

            IRectangle sourceRectangle = Source.GetDrawnRectagle().ToCenteredRectangle();
            Vector2 sourcePatchOrigin = visibleRectangle.Origin - sourceRectangle.Size.Integrate(sourceRectangle.Size.Discretize(visibleRectangle.Origin - drawnRectangle.Origin)) + sourceRectangle.Origin;

            float y = 0;
            while (y < visibleRectangle.Height)
            {
                float sourceRemainingHeight = sourceRectangle.Height - sourcePatchOrigin.Y;
                float destinationRemainingHeight = visibleRectangle.Height - y;
                float sourcePatchHeight = MathHelper.Min(sourceRemainingHeight, destinationRemainingHeight);

                float x = 0;
                while (x < visibleRectangle.Width)
                {
                    float sourceRemainingWidth = sourceRectangle.Width - sourcePatchOrigin.X;
                    float destinationRemainingWidth = visibleRectangle.Width - x;
                    float sourcePatchWidth = MathHelper.Min(sourceRemainingWidth, destinationRemainingWidth);

                    Vector2 position = visibleRectangle.Origin + new Vector2(x, y);
                    IRectangle sourcePatch = new OriginRectangle(sourcePatchOrigin, new Vector2(sourcePatchWidth, sourcePatchHeight));

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToStruct(), SpriteTransformer.Color, 0,
                            Vector2.Zero, SpriteTransformer.Scale, SpriteTransformer.Effects, Depth);
                    else
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToStruct(), Color.White, 0,
                            Vector2.Zero, 1f, SpriteEffects.None, Depth);

                    x += sourcePatchWidth;
                    sourcePatchOrigin.X = sourceRectangle.Origin.X;
                }

                y += sourcePatchHeight;
                sourcePatchOrigin.Y = sourceRectangle.Origin.Y;
            }
        }
    }
}