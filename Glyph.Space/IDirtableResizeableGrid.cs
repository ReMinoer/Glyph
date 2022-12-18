namespace Glyph.Space
{
    public interface IDirtableResizeableGrid<T> : IDirtableGrid<T>, IResizeableGrid<T>
        where T : class, IDirtableGridCell<T>
    {
    }
}