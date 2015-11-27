namespace Glyph.Composition.Layers
{
    public interface ILayerManager : IUpdate
    {
        ILayerRoot GetLayerRoot(IGlyphComponent component);
    }
}