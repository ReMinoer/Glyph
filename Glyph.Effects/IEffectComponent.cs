using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Effects
{
    public interface IEffectComponent : IComponent<IEffectComponent, IEffectParent>, IDisposable
    {
        bool Enabled { get; set; }
        string Name { get; set; }
        void Initialize();
        void LoadContent(ContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Prepare(IDrawer drawer);
        void Apply(IDrawer drawer);
    }
}