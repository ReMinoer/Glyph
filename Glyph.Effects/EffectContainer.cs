using System;
using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public class EffectContainer : EffectContainer<IEffectComponent>, IEffectContainer
    {
        protected EffectContainer(int size)
            : base(size)
        {
        }
    }

    public class EffectContainer<TComponent> : Container<IEffectComponent, IEffectParent, TComponent>, IEffectContainer<TComponent>
        where TComponent : class, IEffectComponent
    {
        public bool Enabled { get; set; }

        protected EffectContainer(int size)
            : base(size)
        {
        }

        public void Initialize()
        {
            foreach (TComponent component in this)
                component.Initialize();
        }

        public void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            foreach (TComponent component in this)
                component.LoadContent(contentLibrary, graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            foreach (TComponent component in this)
                component.Update(gameTime);
        }

        public void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            foreach (TComponent component in this)
                component.Prepare(spriteBatch, graphicsDevice);
        }

        public void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (TComponent component in this)
                component.Dispose();
        }
    }
}