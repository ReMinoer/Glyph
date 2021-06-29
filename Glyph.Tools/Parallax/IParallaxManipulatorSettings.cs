using Glyph.Animation.Parallax;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Parallax
{
    public interface IParallaxManipulatorSettings
    {
        ParallaxRoot ParallaxRoot { get; }
        TopLeftRectangle MainLayerRectangle { get; }
        Vector2 DisplayedSize { get; }
    }
}