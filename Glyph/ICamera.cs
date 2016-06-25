using Microsoft.Xna.Framework;

namespace Glyph
{
    public interface ICamera
    {
        Vector2 Position { get; }
        float Rotation { get; }
        float Zoom { get; set; }
        Matrix Matrix { get; }
    }
}