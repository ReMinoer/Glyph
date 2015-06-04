using System;
using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public class EffectSynthesizer<TInput> : Synthesizer<IEffect, TInput>, IEffectSynthesizer<TInput>
        where TInput : IEffect
    {
        public bool Enabled { get; set; }

        protected EffectSynthesizer(int size)
            : base(size)
        {
        }

        public void Initialize()
        {
            foreach (TInput component in Components)
                component.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            foreach (TInput component in Components)
                component.LoadContent(contentLibrary, graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            foreach (TInput component in Components)
                component.Update(gameTime);
        }

        public void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            foreach (TInput component in Components)
                component.Prepare(spriteBatch, graphicsDevice);
        }

        public void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (TInput component in Components)
                component.Dispose();
        }
    }
}