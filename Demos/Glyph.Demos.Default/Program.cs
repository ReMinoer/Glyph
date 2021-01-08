using Glyph.Content;
using Glyph.Engine;
using Microsoft.Extensions.Logging.Abstractions;

namespace Glyph.Demos.Default
{
    static public class Program
    {
        static public void Main()
        {
            using (var game = new GlyphGame(NullLogger.Instance, _ => new UnusedContentLibrary()))
                game.Run();
        }
    }
}