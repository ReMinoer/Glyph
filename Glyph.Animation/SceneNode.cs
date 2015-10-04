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
        private Matrix3X3 _matrix;

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
                Transformation.SetTranslation(value);
                Refresh();
            }
        }

        public float LocalRotation
        {
            get { return Transformation.Rotation; }
            set
            {
                Transformation.SetRotation(value);
                Refresh();
            }
        }

        public float LocalScale
        {
            get { return Transformation.Scale; }
            set
            {
                Transformation.SetScale(value);
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
                LocalPosition = ParentNode.Matrix.Inverse * value;
            }
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                LocalRotation = value - ParentNode.Rotation;
            }
        }

        public float Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                LocalScale = value / ParentNode.Scale;
            }
        }

        public Matrix3X3 Matrix
        {
            get
            {
                return _matrix;
            }
        }

        public SceneNode()
        {
            _childrenNodes = new List<SceneNode>();
            Transformation = Transformation.Identity;
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
                _matrix = LocalMatrix;

                foreach (SceneNode childNode in _childrenNodes)
                    childNode.Refresh();
            }
            else
            {
                _position = ParentNode.Transformation.Matrix * LocalPosition;
                _rotation = ParentNode.Rotation + LocalRotation;
                _scale = ParentNode.Scale * LocalScale;
                _matrix = ParentNode.Matrix * LocalMatrix;

                foreach (SceneNode childNode in _childrenNodes)
                    childNode.Refresh();
            }
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