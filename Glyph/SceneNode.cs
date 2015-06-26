using Microsoft.Xna.Framework;

namespace Glyph
{
    [SinglePerParent]
    public class SceneNode : GlyphComponent
    {
        private SceneNode _parentNode;
        private Transformation _transformation;
        private Vector2 _position;
        private float _rotation;
        private float _scale;
        private Matrix3X3 _matrix;
        private bool _hasChange;

        public SceneNode ParentNode
        {
            get { return _parentNode; }
            set
            {
                _parentNode = value;
                Refresh();
            }
        }

        public Transformation Transformation
        {
            get { return _transformation; }
            set
            {
                _transformation = value;
                _hasChange = true;
            }
        }

        public Vector2 LocalPosition
        {
            get { return Transformation.Translation; }
            set
            {
                Transformation.SetTranslation(value);
                _hasChange = true;
            }
        }

        public float LocalRotation
        {
            get { return Transformation.Rotation; }
            set
            {
                Transformation.SetRotation(value);
                _hasChange = true;
            }
        }

        public float LocalScale
        {
            get { return Transformation.Scale; }
            set
            {
                Transformation.SetScale(value);
                _hasChange = true;
            }
        }

        public Matrix3X3 LocalMatrix
        {
            get { return Transformation.Matrix; }
        }

        public Vector2 Position
        {
            get
            {
                Refresh();
                return _position;
            }
            set
            {
                Refresh();
                _position = ParentNode.Matrix.Inverse * value;
            }
        }

        public float Rotation
        {
            get
            {
                Refresh();
                return _rotation;
            }
            set
            {
                Refresh();
                LocalRotation = value - ParentNode.Rotation;
            }
        }

        public float Scale
        {
            get
            {
                Refresh();
                return _scale;
            }
            set
            {
                Refresh();
                LocalScale = value / ParentNode.Scale;
            }
        }

        public Matrix3X3 Matrix
        {
            get
            {
                Refresh();
                return _matrix;
            }
        }

        public SceneNode()
        {
            Transformation = Transformation.Identity;
            Refresh();
        }

        private void Refresh()
        {
            if (!ChangeInParents())
                return;

            ParentNode.Refresh();

            _position = ParentNode.Transformation.Matrix * LocalPosition;
            _rotation = ParentNode.Rotation + LocalRotation;
            _scale = ParentNode.Scale * LocalScale;
            _matrix = ParentNode.Matrix * LocalMatrix;

            _hasChange = false;
        }

        private bool ChangeInParents()
        {
            if (ParentNode == null)
                return false;

            return ParentNode._hasChange || ParentNode.ChangeInParents();
        }
    }
}