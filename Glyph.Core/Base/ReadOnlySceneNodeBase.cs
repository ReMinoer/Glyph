using System;
using Diese;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core.Base
{
    public abstract class ReadOnlySceneNodeBase : ISceneNode
    {
        protected abstract ISceneNode SceneNode { get; }

        public ISceneNode ParentNode => SceneNode.ParentNode;
        public IReadOnlyObservableList<ISceneNode> Children => SceneNode.Children;
        public Vector2 Position => SceneNode.Position;
        Vector2 ITransformation.Translation => Position;
        public float Rotation => SceneNode.Rotation;
        public float Scale => SceneNode.Scale;
        public float Depth => SceneNode.Depth;
        public Matrix3X3 Matrix => SceneNode.Matrix;
        public Transformation Transformation => SceneNode.Transformation;
        public Vector2 LocalPosition => SceneNode.LocalPosition;
        public float LocalRotation => SceneNode.LocalRotation;
        public float LocalScale => SceneNode.LocalScale;
        public float LocalDepth => SceneNode.LocalDepth;
        public Matrix3X3 LocalMatrix => SceneNode.LocalMatrix;
        public Transformation LocalTransformation => SceneNode.LocalTransformation;

        public event EventHandler TransformationChanged
        {
            add => SceneNode.TransformationChanged += value;
            remove => SceneNode.TransformationChanged -= value;
        }

        public event EventHandler DepthChanged
        {
            add => SceneNode.DepthChanged += value;
            remove => SceneNode.DepthChanged -= value;
        }

        public event EventHandler<ISceneNode> ParentNodeChanged
        {
            add => SceneNode.ParentNodeChanged += value;
            remove => SceneNode.ParentNodeChanged -= value;
        }

        public bool Represent(IRepresentative<ISceneNode> other)
        {
            return SceneNode == other;
        }
        
        public void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World) => SceneNode.SetParent(parent, childStaticReferential);
        public Vector2 GetPosition(Referential referential) => SceneNode.GetPosition(referential);
        public float GetRotation(Referential referential) => SceneNode.GetRotation(referential);
        public float GetScale(Referential referential) => SceneNode.GetScale(referential);
        public float GetDepth(Referential referential) => SceneNode.GetDepth(referential);

        public Vector2 Transform(Vector2 position) => SceneNode.Transform(position);
        public Vector2 InverseTransform(Vector2 position) => SceneNode.InverseTransform(position);
        public ITransformation Transform(ITransformation transformation) => SceneNode.Transform(transformation);
        public ITransformation InverseTransform(ITransformation transformation) => SceneNode.InverseTransform(transformation);
        
        void ISceneNode.LinkChild(ISceneNode child, Referential childStaticReferential) => SceneNode.LinkChild(child, childStaticReferential);
        void ISceneNode.UnlinkChild(ISceneNode child) => SceneNode.UnlinkChild(child);
        void ISceneNode.Refresh() => SceneNode.Refresh();

        public override int GetHashCode() => SceneNode.GetHashCode();
    }
}