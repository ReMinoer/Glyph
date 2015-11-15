using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface IArea : IGlyphComponent
    {
        bool ContainsPoint(Vector2 point);
    }
}