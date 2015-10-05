using System;
using Diese.Composition;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Composition
{
    public interface IGlyphComponent : IComponent<IGlyphComponent, IGlyphParent>, IDisposable
    {
        void Initialize();
    }

    public interface IEnableable : IGlyphComponent
    {
        bool Enabled { get; set; }
    }

    public interface ILoadContent : IGlyphComponent
    {
        void LoadContent(ContentLibrary contentLibrary);
    }

    public interface IUpdate : IGlyphComponent
    {
        void Update(ElapsedTime elapsedTime);
    }

    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; }
        void Draw(SpriteBatch spriteBatch);
    }
}