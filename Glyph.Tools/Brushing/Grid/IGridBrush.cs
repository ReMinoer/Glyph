using Simulacra.Utils;

namespace Glyph.Tools.Brushing.Grid
{
    public interface IGridBrush<TCell, in TPaint> : IBrush<IWriteableArray<TCell>, IGridBrushArgs, TPaint>
        where TPaint : IPaint
    {
    }
}