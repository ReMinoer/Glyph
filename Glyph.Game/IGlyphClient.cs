using Glyph.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Glyph.Game
{
    public interface IGlyphClient : IInputClient
    {
        GraphicsDevice GraphicsDevice { get; }
    }
}