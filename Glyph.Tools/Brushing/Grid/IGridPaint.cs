using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridPaint<TCell> : IPaint<IWriteableArray<TCell>, IGridBrushArgs>
    {
    }
}