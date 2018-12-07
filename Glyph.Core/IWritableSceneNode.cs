using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public interface IWritableSceneNode : ISceneNode, IFlipable
    {
        new Vector2 Position { get; set; }
        new float Rotation { get; set; }
        new float Scale { get; set; }
        new float Depth { get; set; }
        new Vector2 LocalPosition { get; set; }
        new float LocalRotation { get; set; }
        new float LocalScale { get; set; }
        new float LocalDepth { get; set; }
        new Transformation LocalTransformation { get; set; }
        void SetPosition(Referential referential, Vector2 value);
        void SetRotation(Referential referential, float value);
        void SetScale(Referential referential, float value);
        void SetDepth(Referential referential, float value);
    }
}