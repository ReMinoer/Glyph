using System;
using Diese.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Effects
{
    public interface IEffectComponent : IComponent<IEffectComponent, IEffectParent>, IDisposable
    {
        bool Enabled { get; set; }
        void Initialize();
        void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Prepare(IDrawer drawer);
        void Apply(IDrawer drawer);
    }
}