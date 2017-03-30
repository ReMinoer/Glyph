using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class TexturingRenderer : RendererBase
    {
        private readonly SceneNode _sceneNode;
        private readonly FillingRectangle _fillingRectangle;

        protected override float DepthProtected
        {
            get { return _sceneNode.Depth; }
        }

        public TexturingRenderer(SceneNode sceneNode, FillingRectangle fillingRectangle, ISpriteSource source)
            : base(source)
        {
            _sceneNode = sceneNode;
            _fillingRectangle = fillingRectangle;
        }

        protected override void Render(IDrawer drawer)
        {
            TopLeftRectangle cameraRectangle = drawer.DisplayedRectangle;
            TopLeftRectangle drawnRectangle = new CenteredRectangle
            {
                Center = _sceneNode.Position + _fillingRectangle.Rectangle.Center,
                Size = _fillingRectangle.Rectangle.Size
            };

            TopLeftRectangle visibleRectangle;
            if (!drawnRectangle.Intersects(cameraRectangle, out visibleRectangle))
                return;

            TopLeftRectangle sourceRectangle = Source.GetDrawnRectagle().ToFloats();
            Vector2 sourcePatchInit = visibleRectangle.Position - sourceRectangle.Size.Integrate(sourceRectangle.Size.Discretize(visibleRectangle.Position - drawnRectangle.Position)) + sourceRectangle.Position;
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

                    Vector2 position = visibleRectangle.Position + new Vector2(x, y);
                    var sourcePatch = new TopLeftRectangle(sourcePatchOrigin, new Vector2(sourcePatchWidth, sourcePatchHeight));

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToIntegers(), SpriteTransformer.Color, 0,
                            Vector2.Zero, SpriteTransformer.Scale, SpriteTransformer.Effects, _sceneNode.Depth);
                    else
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position, sourcePatch.ToIntegers(), Color.White, 0,
                            Vector2.Zero, 1f, SpriteEffects.None, _sceneNode.Depth);

                    x += sourcePatchWidth;
                    sourcePatchOrigin.X = sourceRectangle.Position.X;
                }

                y += sourcePatchHeight;
                sourcePatchOrigin.Y = sourceRectangle.Position.Y;
            }
        }
    }
}