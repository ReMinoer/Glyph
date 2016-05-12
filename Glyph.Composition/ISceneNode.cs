using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Composition
{
    public interface ISceneNode
    {
        IGlyphParent Parent { get; }
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
        void LinkChild(ISceneNode child, Referential childStaticReferential = Referential.World);
        void UnlinkChild(ISceneNode child);
        void Refresh();
    }
}