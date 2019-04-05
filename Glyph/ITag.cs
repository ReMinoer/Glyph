using Diese.Collections;

namespace Glyph
{
    public interface ITag<TTaggable> : ITracker<TTaggable>
        where TTaggable : class
    {
        string Name { get; }
    }
}