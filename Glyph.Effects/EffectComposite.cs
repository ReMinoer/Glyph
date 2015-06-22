using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public abstract class EffectComposite : OrderedComposite<IEffect, IEffectParent>, IEffectComposite
    {
        public bool Enabled { get; set; }
        public abstract void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);

        public virtual void Initialize()
        {
            foreach (IEffect effect in Components)
                effect.Initialize();
        }

        public virtual void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            foreach (IEffect effect in Components)
                effect.LoadContent(contentLibrary, graphicsDevice);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (IEffect effect in Components)
                effect.Update(gameTime);
        }

        public virtual void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            foreach (IEffect effect in Components)
                effect.Prepare(spriteBatch, graphicsDevice);
        }

        public virtual void Dispose()
        {
            foreach (IEffect effect in Components)
                effect.Dispose();
        }
    }
}