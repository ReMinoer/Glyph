using Glyph.Math.Shapes;

namespace Glyph.UI
{
    public interface IFrame : IControl
    {
        OriginRectangle Bounds { get; }
    }
}