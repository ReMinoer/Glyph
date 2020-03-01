using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class SpriteBatchContext : ICloneable
    {
        public SpriteSortMode SpriteSortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Effect Effect { get; set; }
        public Matrix? TransformMatrix { get; set; }

        static public SpriteBatchContext None => null;
        static public SpriteBatchContext Default => new SpriteBatchContext();

        public SpriteBatchContext Clone()
        {
            return new SpriteBatchContext
            {
                SpriteSortMode = SpriteSortMode,
                BlendState = BlendState,
                SamplerState = SamplerState,
                DepthStencilState = DepthStencilState,
                RasterizerState = RasterizerState,
                Effect = Effect,
                TransformMatrix = TransformMatrix
            };
        }

        object ICloneable.Clone() => Clone();
    }
}