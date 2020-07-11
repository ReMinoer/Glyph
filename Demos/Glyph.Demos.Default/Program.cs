using Glyph.Engine;

namespace Glyph.Demos.Default
{
    static public class Program
    {
        static public void Main()
        {
            using (var game = new GlyphGame(_ => new UnusedContentLibrary()))
                game.Run();
        }
    }
}