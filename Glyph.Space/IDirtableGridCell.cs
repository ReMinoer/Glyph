using Microsoft.Xna.Framework;

namespace Glyph.Space
{
    public interface IDirtableGridCell<T>
        where T : class, IDirtableGridCell<T>
    {
        IDirtableGrid<T> Grid { get; set; }
        Point GridPoint { get; set; }
    }
}