using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Diese;
using Diese.Collections;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Core
{
    [SinglePerParent]
    public class SceneNode : GlyphComponent, IWritableSceneNode
    {
        private readonly ObservableCollection<ISceneNode> _childrenNodes;
        private readonly IReadOnlyList<ISceneNode> _readOnlyChildrenNodes;
        private Transformation _transformation;
        private Vector2 _position;
        private float _rotation;
        private float _scale;
        private float _localDepth;
        private float _depth;
        private bool _isRoot;
        public ISceneNode ParentNode { get; private set; }
        public Matrix3X3 Matrix { get; private set; }

        public IReadOnlyCollection<ISceneNode> Children
        {
            get { return _readOnlyChildrenNodes; }
        }

        public Transformation LocalTransformation
        {
            get { return _transformation; }
            set
            {
                _transformation = value;
                Refresh(Referential.Local);
            }
        }

        public Vector2 LocalPosition
        {
            get { return LocalTransformation.Translation; }
            set
            {
                LocalTransformation.Translation = value;
                Refresh(Referential.Local);
            }
        }

        public float LocalRotation
        {
            get { return LocalTransformation.Rotation; }
            set
            {
                LocalTransformation.Rotation = value;
                Refresh(Referential.Local);
            }
        }

        public float LocalScale
        {
            get { return LocalTransformation.Scale; }
            set
            {
                LocalTransformation.Scale = value;
                Refresh(Referential.Local);
            }
        }

        public float LocalDepth
        {
            get { return _localDepth; }
            set
            {
                _localDepth = value;
                Refresh(Referential.Local);
            }
        }

        public Matrix3X3 LocalMatrix
        {
            get { return LocalTransformation.Matrix; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                SetWorldPosition(value);
                Refresh(Referential.Local);
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                SetWorldRotation(value);
                Refresh(Referential.Local);
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                SetWorldScale(value);
                Refresh(Referential.Local);
            }
        }

        public float Depth
        {
            get { return _depth; }
            set
            {
                SetWorldDepth(value);
                Refresh(Referential.Local);
            }
        }

        public Transformation Transformation
        {
            get { return new Transformation(Position, Rotation, Scale); }
            set { SetValues(value.Translation, value.Rotation, value.Scale, null, Referential.World); }
        }

        public event Action<SceneNode> Refreshed;

        public SceneNode()
        {
            _childrenNodes = new ObservableCollection<ISceneNode>();
            _readOnlyChildrenNodes = new System.Collections.ObjectModel.ReadOnlyObservableCollection<ISceneNode>(_childrenNodes);

            LocalTransformation = Transformation.Identity;
        }

        public SceneNode(ISceneNode parentNode)
            : this()
        {
            SetParent(parentNode, Referential.Local);
        }

        public override void Initialize()
        {
            if (_isRoot || ParentNode != null || Parent == null)
                return;

            if (Parent.ParentQueue().SelectMany(x => x.Components).AnyOfType(out SceneNode parentNode))
                SetParent(parentNode, Referential.Local);
        }

        public override void Dispose()
        {
            foreach (ISceneNode childNode in _childrenNodes.ToArray())
                childNode.SetParent(null);

            SetParent(null);
        }

        public void SetValues(Vector2? position, float? rotation, float? scale, float? depth, Referential childStaticReferential)
        {
            switch (childStaticReferential)
            {
                case Referential.Local:
                    LocalTransformation.RefreshMatrix(position, rotation, scale);
                    if (depth.HasValue)
                        _localDepth = depth.Value;
                    break;
                case Referential.World:
                    if (position.HasValue)
                        SetWorldPosition(position.Value);
                    if (rotation.HasValue)
                        SetWorldRotation(rotation.Value);
                    if (scale.HasValue)
                        SetWorldScale(scale.Value);
                    if (depth.HasValue)
                        SetWorldDepth(depth.Value);
                    break;
            }
            Refresh(Referential.Local);
        }

        private void SetWorldPosition(Vector2 value)
        {
            if (ParentNode != null)
                LocalTransformation.Translation = ParentNode.Matrix.Inverse * value;
            else
                LocalTransformation.Translation = value;
        }

        private void SetWorldRotation(float value)
        {
            if (ParentNode != null)
                LocalTransformation.Rotation = value - ParentNode.Rotation;
            else
                LocalTransformation.Rotation = value;
        }

        private void SetWorldScale(float value)
        {
            if (ParentNode != null)
                LocalTransformation.Scale = value / ParentNode.Scale;
            else
                LocalTransformation.Scale = value;
        }

        private void SetWorldDepth(float value)
        {
            if (ParentNode != null)
                _localDepth = value - ParentNode.Depth;
            else
                _localDepth = value;
        }

        public void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World)
        {
            ParentNode?.UnlinkChild(this);
            ParentNode = parent;

            _isRoot = ParentNode == null;
            if (!_isRoot)
                ParentNode?.LinkChild(this, childStaticReferential);

            Refresh(childStaticReferential);
        }

        public void MakesRoot()
        {
            SetParent(null, Referential.Local);
        }

        public Vector2 GetPosition(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return Position;
                case Referential.Local: return LocalPosition;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetRotation(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return Rotation;
                case Referential.Local: return LocalRotation;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetScale(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return Scale;
                case Referential.Local: return LocalScale;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetDepth(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return Depth;
                case Referential.Local: return LocalDepth;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetPosition(Referential referential, Vector2 value)
        {
            switch (referential)
            {
                case Referential.World: Position = value; break;
                case Referential.Local: LocalPosition = value; break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetRotation(Referential referential, float value)
        {
            switch (referential)
            {
                case Referential.World: Rotation = value; break;
                case Referential.Local: LocalRotation = value; break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetScale(Referential referential, float value)
        {
            switch (referential)
            {
                case Referential.World: Scale = value; break;
                case Referential.Local: LocalScale = value; break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void SetDepth(Referential referential, float value)
        {
            switch (referential)
            {
                case Referential.World: Depth = value; break;
                case Referential.Local: LocalDepth = value; break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void Flip(Axes axes)
        {
            LocalTransformation.Flip(axes);
            Refresh(Referential.Local);
        }

        public bool Represent(IRepresentative<ISceneNode> other)
        {
            return this == other;
        }

        private void Refresh(Referential childStaticReferential)
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
                switch (childStaticReferential)
                {
                    case Referential.Local:
                        _position = ParentNode.Matrix * LocalPosition;
                        _rotation = MathHelper.WrapAngle(ParentNode.Rotation + LocalRotation);
                        _scale = ParentNode.Scale * LocalScale;
                        _depth = ParentNode.Depth + LocalDepth;
                        break;
                    case Referential.World:
                        _transformation.RefreshMatrix(ParentNode.Matrix.Inverse * _position, _rotation - ParentNode.Rotation, _scale / ParentNode.Scale);
                        _localDepth = _depth - ParentNode.Depth;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                Matrix = ParentNode.Matrix * LocalMatrix;
            }

            foreach (ISceneNode childNode in _childrenNodes)
                childNode.Refresh();

            Refreshed?.Invoke(this);
        }

        void ISceneNode.LinkChild(ISceneNode child, Referential childStaticReferential)
        {
            if (_childrenNodes.Contains(child))
                throw new InvalidOperationException("Parent already have this SceneNode as a child !");

            if (!child.ParentNode.Represent(this))
                child.SetParent(this, childStaticReferential);
            else
                _childrenNodes.Add(child);
        }

        void ISceneNode.UnlinkChild(ISceneNode child)
        {
            _childrenNodes.Remove(child);
        }

        void ISceneNode.Refresh()
        {
            Refresh(Referential.Local);
        }

        public Vector2 Transform(Vector2 position) => new Transformation(Position, Rotation, Scale).Transform(position);
        public Vector2 InverseTransform(Vector2 position) => new Transformation(Position, Rotation, Scale).InverseTransform(position);
        public Transformation Transform(Transformation transformation) => new Transformation(Position, Rotation, Scale).Transform(transformation);
        public Transformation InverseTransform(Transformation transformation) => new Transformation(Position, Rotation, Scale).InverseTransform(transformation);
    }
}