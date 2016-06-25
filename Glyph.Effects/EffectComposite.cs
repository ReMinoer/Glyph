using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public abstract class EffectComposite : EffectComposite<IEffectComponent>, IEffectComposite
    {
    }

    public abstract class EffectComposite<TComponent> : OrderedComposite<IEffectComponent, IEffectParent, TComponent>, IEffectComposite<TComponent>
        where TComponent : class, IEffectComponent
    {
        public bool Enabled { get; set; }
        public abstract void Apply(IDrawer drawer);

        public virtual void Initialize()
        {
            foreach (TComponent effect in this)
                effect.Initialize();
        }

        public virtual void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice)
        {
            foreach (TComponent effect in this)
                effect.LoadContent(contentLibrary, graphicsDevice);
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (TComponent effect in this)
                effect.Update(gameTime);
        }

        public virtual void Prepare(IDrawer drawer)
        {
            foreach (TComponent effect in this)
                effect.Prepare(drawer);
        }

        public virtual void Dispose()
        {
            foreach (TComponent effect in this)
                effect.Dispose();
        }
    }
}