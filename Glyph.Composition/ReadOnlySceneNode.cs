using System.Collections.Generic;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Composition
{
    public class ReadOnlySceneNode : ISceneNode
    {
        private readonly ISceneNode _sceneNode;

        public IGlyphParent Parent
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

        public bool Equals(ISceneNode other)
        {
            return _sceneNode == other;
        }

        void ISceneNode.LinkChild(ISceneNode child, Referential childStaticReferential = Referential.World)
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