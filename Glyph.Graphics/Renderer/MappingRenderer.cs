using Glyph.Math;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public delegate bool RenderingBehaviour<TData>(TData caseData, MappingRenderer<TData> mappingRenderer);

    public class MappingRenderer<TData> : RendererBase
    {
        public ISpriteSheet SpriteSheet { get; }
        public IGrid<TData> Grid { get; set; }
        public Transformation Transformation { get; set; }
        public float Depth { get; set; }
        public RenderingBehaviour<TData> RenderingBehaviour { get; set; }

        protected override float DepthProtected
        {
            get { return Depth; }
        }

        public MappingRenderer(ISpriteSheet spriteSheet)
            : base(spriteSheet)
        {
            SpriteSheet = spriteSheet;
            Transformation = Transformation.Identity;
        }

        protected override void Render(IDrawer drawer)
        {
            for (int i = 0; i < Grid.Dimension.Rows; i++)
                for (int j = 0; j < Grid.Dimension.Columns; j++)
                {
                    Vector2 position = Grid.ToWorldPoint(i, j) + Grid.Delta / 2;

                    if (!RenderingBehaviour(Grid[i, j], this))
                        continue;

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, SpriteTransformer.Color,
                            Transformation.Rotation, SpriteTransformer.Origin, Transformation.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, Depth);
                    else
                    {
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, Color.White,
                            Transformation.Rotation, Source.GetDefaultOrigin(), Transformation.Scale, SpriteEffects.None, Depth);
                    }
                }
        }
    }
}