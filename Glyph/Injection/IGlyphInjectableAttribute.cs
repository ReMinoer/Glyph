using Niddle;

namespace Glyph.Injection
{
    public interface IGlyphInjectableAttribute : IInjectableAttribute
    {
        GlyphInjectableTargets Targets { get; }
    }
}