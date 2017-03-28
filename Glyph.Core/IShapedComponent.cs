using Glyph.Math;

namespace Glyph.Core
{
    public interface IShapedComponent : IBoxedComponent
    {
        IShape Shape { get; }
    }

    public interface IShapedComponent<out T> : IShapedComponent
        where T : IShape
    {
        new T Shape { get; }
    }
}