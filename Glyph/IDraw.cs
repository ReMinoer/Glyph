using System;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDraw : IGlyphComponent
    {
        bool Visible { get; }
        event EventHandler VisibleChanged;
        void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
    }
}