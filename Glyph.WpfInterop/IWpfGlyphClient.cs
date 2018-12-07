using Glyph.Engine;
using MonoGame.Framework.WpfInterop;

namespace Glyph.WpfInterop
{
    public interface IWpfGlyphClient : IGlyphClient
    {
        IGameRunner Runner { get; set; }
    }
}