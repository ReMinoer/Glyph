using System;
using Diese;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph
{
    public interface ISceneNode : IRepresentative<ISceneNode>, ITransformation
    {
        ISceneNode ParentNode { get; }
        IReadOnlyObservableList<ISceneNode> Children { get; }
        Vector2 Position { get; }
        float Depth { get; }
        Transformation Transformation { get; }
        Vector2 LocalPosition { get; }
        float LocalRotation { get; }
        float LocalScale { get; }
        float LocalDepth { get; }
        Matrix3X3 LocalMatrix { get; }
        Transformation LocalTransformation { get; }
        void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World);
        Vector2 GetPosition(Referential referential);
        float GetRotation(Referential referential);
        float GetScale(Referential referential);
        float GetDepth(Referential referential);
        void LinkChild(ISceneNode child, Referential childStaticReferential = Referential.World);
        void UnlinkChild(ISceneNode child);
        void Refresh();
        event EventHandler DepthChanged;
        event EventHandler<ISceneNode> ParentNodeChanged;
    }
}