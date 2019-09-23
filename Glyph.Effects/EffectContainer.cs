using System;
using System.Linq;
using System.Threading.Tasks;
using Diese;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Effects
{
    public class EffectContainer : EffectContainer<IEffectComponent>
    {
        protected EffectContainer()
        {
        }
    }

    public class EffectContainer<TComponent> : Container<IEffectComponent, IEffectContainer, TComponent>, IEffectContainer<TComponent>
        where TComponent : class, IEffectComponent
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }

        protected EffectContainer()
        {
            Name = GetType().GetDisplayName();
        }

        public void Initialize()
        {
            foreach (TComponent component in Components)
                component.Initialize();
        }

        public async Task LoadContent(IContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            await Task.WhenAll(Components.Select(x => x.LoadContent(contentLibrary, graphicsDevice)));
        }

        public void Update(GameTime gameTime)
        {
            foreach (TComponent component in Components)
                component.Update(gameTime);
        }

        public void Prepare(IDrawer drawer)
        {
            foreach (TComponent component in Components)
                component.Prepare(drawer);
        }

        public void Apply(IDrawer drawer)
        {
            throw new InvalidOperationException();
        }

        public void Dispose()
        {
            foreach (TComponent component in Components)
                component.Dispose();
        }
    }
}