using System;
using Glyph.Space;

namespace Glyph.Physics.Colliders
{
    public interface IGridCollider : ICollider
    {
        IGrid Grid { get; }
        bool IsCollidableCase(int i, int j);
    }
}