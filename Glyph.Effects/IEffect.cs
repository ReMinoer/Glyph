using System;
using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public interface IEffect : IComponent<IEffect, IEffectParent>, IDisposable
    {
        bool Enabled { get; set; }
        void Initialize();
        void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
        void Apply(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
    }
}