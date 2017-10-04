using System.Collections.Generic;
using Diese;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class ReadOnlySceneNode : ISceneNode
    {
        private readonly ISceneNode _sceneNode;

        public IGlyphContainer Parent
        {
            get { return _sceneNode.Parent; }
        }

        public ISceneNode ParentNode
        {
            get { return _sceneNode.ParentNode; }
        }

        public IEnumerable<ISceneNode> Children
        {
            get { return _sceneNode.Children; }
        }

        public Vector2 Position
        {
            get { return _sceneNode.Position; }
        }

        public float Rotation
        {
            get { return _sceneNode.Rotation; }
        }

        public float Scale
        {
            get { return _sceneNode.Scale; }
        }

        public float Depth
        {
            get { return _sceneNode.Depth; }
        }

        public Vector2 LocalPosition
        {
            get { return _sceneNode.LocalPosition; }
        }

        public float LocalRotation
        {
            get { return _sceneNode.LocalRotation; }
        }

        public float LocalScale
        {
            get { return _sceneNode.LocalScale; }
        }

        public float LocalDepth
        {
            get { return _sceneNode.LocalDepth; }
        }

        public Matrix3X3 Matrix
        {
            get { return _sceneNode.Matrix; }
        }

        public Matrix3X3 LocalMatrix
        {
            get { return _sceneNode.LocalMatrix; }
        }

        public Transformation Transformation
        {
            get { return _sceneNode.Transformation; }
        }

        public ReadOnlySceneNode(ISceneNode sceneNode)
        {
            _sceneNode = sceneNode;
        }

        public void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World)
        {
            _sceneNode.SetParent(parent, childStaticReferential);
        }

        public Vector2 GetPosition(Referential referential)
        {
            return _sceneNode.GetPosition(referential);
        }

        public float GetRotation(Referential referential)
        {
            return _sceneNode.GetRotation(referential);
        }

        public float GetScale(Referential referential)
        {
            return _sceneNode.GetScale(referential);
        }

        public float GetDepth(Referential referential)
        {
            return _sceneNode.GetDepth(referential);
        }

        public bool Represent(IRepresentative<ISceneNode> other)
        {
            return _sceneNode == other;
        }

        void ISceneNode.LinkChild(ISceneNode child, Referential childStaticReferential)
        {
            _sceneNode.LinkChild(child, childStaticReferential);
        }

        void ISceneNode.UnlinkChild(ISceneNode child)
        {
            _sceneNode.UnlinkChild(child);
        }

        void ISceneNode.Refresh()
        {
            _sceneNode.Refresh();
        }
    }
}