using Glyph.Space;

namespace Glyph.Core.Colliders
{
    public interface IGridCollider : ICollider
    {
        IGrid Grid { get; }
        bool IsCollidableCase(ICollider collider, int i, int j);
    }
}