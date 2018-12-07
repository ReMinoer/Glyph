using Glyph.Composition;

namespace Glyph.Core.Layers
{
    public interface ILayerRoot : IGlyphComponent
    {
        ILayer Layer { get; }
    }

    public interface ILayerRoot<out TLayer> : ILayerRoot
        where TLayer : class, ILayer
    {
        new TLayer Layer { get; }
    }
}