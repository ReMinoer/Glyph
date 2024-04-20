using Glyph.Composition;

namespace Glyph.Graphics
{
    public interface IEffectRenderer : IGlyphComponent
    {
        void PrepareEffect(IDrawer drawer);
        void ApplyEffect(IDrawer drawer);
    }
}