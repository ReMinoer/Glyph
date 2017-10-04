using Diese;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Effects
{
    public abstract class EffectComponent : Component<IEffectComponent, IEffectContainer>, IEffectComponent
    {
        public virtual bool Enabled { get; set; }
        public string Name { get; set; }

        protected EffectComponent()
        {
            Name = GetType().GetDisplayName();
        }

        public abstract void Initialize();
        public abstract void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        public abstract void Update(GameTime gameTime);
        public abstract void Prepare(IDrawer drawer);
        public abstract void Apply(IDrawer drawer);
        public abstract void Dispose();
    }
}