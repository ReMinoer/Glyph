using Glyph.Composition;
using Glyph.Graphics;
using Microsoft.Xna.Framework;

namespace Glyph.UI
{
    public interface IBorder : IDraw, IBounded
    {
        Color Color { get; set; }
    }
}