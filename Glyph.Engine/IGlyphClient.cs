using Glyph.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Engine
{
    public interface IGlyphClient : IInputClient
    {
        GraphicsDevice GraphicsDevice { get; }
    }
}