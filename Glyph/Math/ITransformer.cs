using System;
using Microsoft.Xna.Framework;

namespace Glyph.Math
{
    public interface ITransformer
    {
        Vector2 Transform(Vector2 position);
        Vector2 InverseTransform(Vector2 position);
        ITransformation Transform(ITransformation transformation);
        ITransformation InverseTransform(ITransformation transformation);
        event EventHandler TransformationChanged;
    }
}