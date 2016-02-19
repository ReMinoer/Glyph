using Glyph.Math;

namespace Glyph.Composition
{
    public interface IShapedObject
    {
        ISceneNode SceneNode { get; }
        IShape Shape { get; }
    }

    public interface IShapedObject<out T> : IShapedObject
        where T : IShape
    {
        new T Shape { get; }
    }
}