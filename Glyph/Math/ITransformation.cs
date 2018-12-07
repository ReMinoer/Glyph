using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface ITransformation : ITransformer, IFlipable
    {
        Vector2 Translation { get; }
        float Rotation { get; }
        float Scale { get; }
        Matrix3X3 Matrix { get; }
    }
}