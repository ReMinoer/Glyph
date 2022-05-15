using Glyph.Composition.Modelization.Base;

namespace Glyph.Composition.Modelization
{
    public interface IHierarchicalData
    {
        SubDataCollection<IGlyphData> SubData { get; }
        SubDataSourceCollection<IGlyphData> SubDataSources { get; }
    }
}