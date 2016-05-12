using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Composition
{
    public interface IWritableSceneNode : ISceneNode
    {
        new Vector2 Position { get; set; }
        new float Rotation { get; set; }
        new float Scale { get; set; }
        new float Depth { get; set; }
        new Vector2 LocalPosition { get; set; }
        new float LocalRotation { get; set; }
        new float LocalScale { get; set; }
        new float LocalDepth { get; set; }
        new Transformation Transformation { get; set; }
    }
}