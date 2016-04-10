using System;
using Glyph.Composition;
using Glyph.Math;
using Glyph.Space;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics.Renderer
{
    public class MappingRenderer<TData> : RendererBase
    {
        public ISpriteSheet SpriteSheet { get; }
        public GridRectangle<TData> Grid { get; set; }
        public Transformation Transformation { get; set; }
        public float Depth { get; set; }
        public Action<TData, MappingRenderer<TData>> RenderingBehaviour { get; set; }

        public MappingRenderer(ISpriteSheet spriteSheet)
            : base(spriteSheet)
        {
            SpriteSheet = spriteSheet;
        }

        protected override void Render(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Grid.GridSize.Rows; i++)
                for (int j = 0; j < Grid.GridSize.Columns; j++)
                {
                    Vector2 position = Grid.ToWorldPoint(i, j) + Grid.Delta / 2;

                    RenderingBehaviour(Grid[i, j], this);

                    if (SpriteTransformer != null)
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, position + Transformation.Translation, Source.Rectangle, SpriteTransformer.Color,
                            Transformation.Rotation, SpriteTransformer.Origin, Transformation.Scale * SpriteTransformer.Scale, SpriteTransformer.Effects, Depth);
                    else
                    {
                        drawer.SpriteBatchStack.Current.Draw(Source.Texture, Transformation.Translation, Source.Rectangle, Color.White,
                            Transformation.Rotation, Source.GetDrawnRectangle().Center.ToVector2(), Transformation.Scale, SpriteEffects.None, Depth);
                    }
                }
        }
    }
}