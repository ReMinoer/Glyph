using Glyph.Math;

namespace Glyph.Composition
{
    public interface IShapedComponent : IGlyphComponent
    {
        ISceneNode SceneNode { get; }
        IShape Shape { get; }
    }

    public interface IShapedComponent<out T> : IShapedComponent
        where T : IShape
    {
        new T Shape { get; }
    }
}