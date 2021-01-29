using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public class SpriteBatchContext : ICloneable
    {
        public SpriteSortMode SpriteSortMode { get; set; } = SpriteSortMode.Deferred;
        public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
        public SamplerState SamplerState { get; set; } = SamplerState.LinearClamp;
        public DepthStencilState DepthStencilState { get; set; } = DepthStencilState.None;
        public RasterizerState RasterizerState { get; set; } = RasterizerState.CullCounterClockwise;
        public Effect Effect { get; set; }
        public Matrix TransformMatrix { get; set; } = Matrix.Identity;

        public SpriteBatchContext()
        {
        }

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