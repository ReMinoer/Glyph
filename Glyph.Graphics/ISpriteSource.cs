using System;
using Glyph.Composition;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Graphics
{
    public interface ISpriteSource : IGlyphComponent, IDisposable
    {
        Texture2D Texture { get; }
        Rectangle? Rectangle { get; }
        Vector2 GetDefaultOrigin();
        Rectangle GetDrawnRectagle();
        event Action<ISpriteSource> Loaded;
    }
}