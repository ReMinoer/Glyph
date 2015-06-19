using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public abstract class EffectComponent : Component<IEffect, IEffectParent>, IEffect
    {
        public virtual bool Enabled { get; set; }
        public abstract void Initialize();
        public abstract void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        public abstract void Update(GameTime gameTime);
        public abstract void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
        public abstract void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
        public abstract void Dispose();
    }
}