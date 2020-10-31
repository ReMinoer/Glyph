using Microsoft.Xna.Framework;
using Simulacra.Utils;

namespace Glyph.Space
{
    public interface IResizeableGrid : IGrid, IResizeableArrayDefinition, INotifyArrayChanged
    {
        new GridDimension Dimension { get; set; }
        new Vector2 Delta { get; set; }
    }

    public interface IResizeableGrid<T> : IResizeableGrid, IWriteableGrid<T>, ITwoDimensionResizeableArray<T>
    {
    }
}