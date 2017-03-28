using Glyph.Composition;

namespace Glyph.Core.Layers
{
    public interface ILayerManager : IUpdate
    {
        ILayerRoot GetLayerRoot(IGlyphComponent component);
    }
}