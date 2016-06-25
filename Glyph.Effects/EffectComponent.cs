using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public abstract class EffectComponent : Component<IEffectComponent, IEffectParent>, IEffectComponent
    {
        public virtual bool Enabled { get; set; }
        public abstract void Initialize();
        public abstract void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        public abstract void Update(GameTime gameTime);
        public abstract void Prepare(IDrawer drawer);
        public abstract void Apply(IDrawer drawer);
        public abstract void Dispose();
    }
}