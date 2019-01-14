using System;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface ITransformer
    {
        Vector2 Transform(Vector2 position);
        Vector2 InverseTransform(Vector2 position);
        Transformation Transform(Transformation transformation);
        Transformation InverseTransform(Transformation transformation);
        event EventHandler TransformationChanged;
    }
}