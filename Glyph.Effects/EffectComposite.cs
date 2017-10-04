using Diese;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Effects
{
    public abstract class EffectComposite : EffectComposite<IEffectComponent>, IEffectComposite
    {
    }

    public abstract class EffectComposite<TComponent> : OrderedComposite<IEffectComponent, IEffectContainer, TComponent>, IEffectComposite<TComponent>
        where TComponent : class, IEffectComponent
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }

        protected EffectComposite()
        {
            Name = GetType().GetDisplayName();
        }

        public virtual void Initialize()
        {
            foreach (TComponent effect in Components)
                effect.Initialize();
        }

        public virtual void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            foreach (TComponent effect in Components)
                effect.LoadContent(contentLibrary, graphicsDevice);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (TComponent effect in Components)
                effect.Update(gameTime);
        }

        public virtual void Prepare(IDrawer drawer)
        {
            foreach (TComponent effect in Components)
                effect.Prepare(drawer);
        }

        public abstract void Apply(IDrawer drawer);

        public virtual void Dispose()
        {
            foreach (TComponent effect in Components)
                effect.Dispose();
        }
    }
}