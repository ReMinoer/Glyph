using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Glyph.Graphics
{
    public interface ITextureMapper
    {
        event EventHandler RefreshRequested;
        Vector2[] GetVertexTextureCoordinates(IReadOnlyList<Vector2> vertices);
    }
}