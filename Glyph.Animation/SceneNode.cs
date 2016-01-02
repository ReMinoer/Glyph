using System;
using System.Collections.Generic;
using Glyph.Composition;
using Microsoft.Xna.Framework;

namespace Glyph.Animation
{
    [SinglePerParent]
    public class SceneNode : GlyphComponent
    {
        private SceneNode _parentNode;
        private readonly List<SceneNode> _childrenNodes;
        private Transformation _transformation;
        private Vector2 _position;
        private float _rotation;
        private float _scale;
        private float _localDepth;
        private float _depth;
        public Matrix3X3 Matrix { get; private set; }
        public event Action<SceneNode> Refreshed;

        public SceneNode ParentNode
        {
            get { return _parentNode; }
            set
            {
                if (_parentNode != null)
                    _parentNode.RemoveChild(this);

                _parentNode = value;

                if (_parentNode != null)
                    _parentNode.AddChild(this);

                Refresh();
            }
        }

        public Transformation Transformation
        {
            get { return _transformation; }
            set
            {
                _transformation = value;
                Refresh();
            }
        }

        public Vector2 LocalPosition
        {
            get { return Transformation.Translation; }
            set
            {
                Transformation.Translation = value;
                Refresh();
            }
        }

        public float LocalRotation
        {
            get { return Transformation.Rotation; }
            set
            {
                Transformation.Rotation = value;
                Refresh();
            }
        }

        public float LocalScale
        {
            get { return Transformation.Scale; }
            set
            {
                Transformation.Scale = value;
                Refresh();
            }
        }

        public float LocalDepth
        {
            get { return _localDepth; }
            set
            {
                _localDepth = value;
                Refresh();
            }
        }

        public Matrix3X3 LocalMatrix
        {
            get { return Transformation.Matrix; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (ParentNode != null)
                    LocalPosition = ParentNode.Matrix.Inverse * value;
                else
                    LocalPosition = value;
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                if (ParentNode != null)
                    LocalRotation = value - ParentNode.Rotation;
                else
                    LocalRotation = value;
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (ParentNode != null)
                    LocalScale = value / ParentNode.Scale;
                else
                    LocalScale = value;
            }
        }

        public float Depth
        {
            get { return _depth; }
            set
            {
                if (ParentNode != null)
                    LocalDepth = value - ParentNode.Depth;
                else
                    LocalDepth = value;
            }
        }

        public SceneNode()
        {
            _childrenNodes = new List<SceneNode>();
            Transformation = Transformation.Identity;
        }

        public SceneNode(SceneNode parentNode)
            : this()
        {
            ParentNode = parentNode;
        }

        public override void Initialize()
        {
            if (Parent != null && Parent.Parent != null)
                ParentNode = Parent.Parent.GetComponent<SceneNode>();
        }

        public override void Dispose()
        {
            foreach (SceneNode childNode in _childrenNodes)
                childNode.ParentNode = null;
        }

        private void Refresh()
        {
            if (ParentNode == null)
            {
                _position = LocalPosition;
                _rotation = LocalRotation;
                _scale = LocalScale;
                _depth = LocalDepth;
                Matrix = LocalMatrix;
            }
            else
            {
                _position = ParentNode.Transformation.Matrix * LocalPosition;
                _rotation = ParentNode.Rotation + LocalRotation;
                _scale = ParentNode.Scale * LocalScale;
                _depth = ParentNode.Depth + LocalDepth;
                Matrix = ParentNode.Matrix * LocalMatrix;
            }

            foreach (SceneNode childNode in _childrenNodes)
                childNode.Refresh();

            if (Refreshed != null)
                Refreshed.Invoke(this);
        }

        private void AddChild(SceneNode child)
        {
            _childrenNodes.Add(child);
        }

        private void RemoveChild(SceneNode child)
        {
            _childrenNodes.Remove(child);
        }
    }
}