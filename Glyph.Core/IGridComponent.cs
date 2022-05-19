using Glyph.Space;

namespace Glyph.Core
{
    public interface IGridComponent : IBoxedComponent
    {
        IGrid Grid { get; }
    }
}