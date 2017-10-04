using System.Collections.Generic;
using Diese;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public interface ISceneNode : IRepresentative<ISceneNode>
    {
        IGlyphContainer Parent { get; }
        ISceneNode ParentNode { get; }
        IEnumerable<ISceneNode> Children { get; }
        Vector2 Position { get; }
        float Rotation { get; }
        float Scale { get; }
        float Depth { get; }
        Vector2 LocalPosition { get; }
        float LocalRotation { get; }
        float LocalScale { get; }
        float LocalDepth { get; }
        Matrix3X3 Matrix { get; }
        Matrix3X3 LocalMatrix { get; }
        Transformation Transformation { get; }
        void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World);
        Vector2 GetPosition(Referential referential);
        float GetRotation(Referential referential);
        float GetScale(Referential referential);
        float GetDepth(Referential referential);
        void LinkChild(ISceneNode child, Referential childStaticReferential = Referential.World);
        void UnlinkChild(ISceneNode child);
        void Refresh();
    }
}