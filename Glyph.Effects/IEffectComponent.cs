using System;
using System.Threading.Tasks;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stave;

namespace Glyph.Effects
{
    public interface IEffectComponent : IComponent<IEffectComponent, IEffectContainer>, IDisposable
    {
        bool Enabled { get; set; }
        string Name { get; set; }
        void Initialize();
        Task LoadContent(IContentLibrary contentLibrary, GraphicsDevice graphicsDevice);
        void Update(GameTime gameTime);
        void Prepare(IDrawer drawer);
        void Apply(IDrawer drawer);
    }
}