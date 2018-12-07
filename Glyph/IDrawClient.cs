using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph
{
    public interface IDrawClient
    {
        Vector2 Size { get; }
        GraphicsDevice GraphicsDevice { get; }
        RenderTarget2D DefaultRenderTarget { get; }
        event Action<Vector2> SizeChanged;
    }
}