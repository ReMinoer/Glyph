using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public interface IPositionController : IAnchoredController
    {
        Vector2 Position { get; set; }
    }
}