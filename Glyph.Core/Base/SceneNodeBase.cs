using System;
using System.Linq;
using Diese;
using Diese.Collections;
using Diese.Collections.Observables;
using Diese.Collections.Observables.ReadOnly;
using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;
using Stave;

namespace Glyph.Core.Base
{
    public class SceneNodeBase : GlyphComponent, ISceneNodeComponent
    {
        protected Transformation _transformation;
        protected Vector2 _position;
        protected float _rotation;
        protected float _scale = 1;

        protected float _localDepth;
        protected float _depth;

        private bool _isRoot;
        private readonly ObservableList<ISceneNode> _childrenNodes;

        public ISceneNode ParentNode { get; private set; }
        public Matrix3X3 Matrix { get; private set; } = Matrix3X3.Identity;
        public IReadOnlyObservableList<ISceneNode> Children { get; }

        Transformation ISceneNode.LocalTransformation => _transformation;
        Transformation ISceneNode.Transformation => new Transformation(_position, _rotation, _scale);
        Vector2 ISceneNode.LocalPosition => _transformation.Translation;
        Vector2 ISceneNode.Position => _position;
        float ISceneNode.LocalRotation => _transformation.Rotation;
        float ISceneNode.Rotation => _rotation;
        float ISceneNode.LocalScale => _transformation.Scale;
        float ISceneNode.Scale => _scale;
        float ISceneNode.LocalDepth => _localDepth;
        float ISceneNode.Depth => _depth;
        Matrix3X3 ISceneNode.LocalMatrix => _transformation.Matrix;

        public float LocalDepth
        {
            get => _localDepth;
            set
            {
                _localDepth = value;
                Refresh();
            }
        }

        public float Depth
        {
            get => _depth;
            set
            {
                SetWorldDepth(value);
                Refresh();
            }
        }

        public bool IsInitialized => _isRoot || ParentNode != null;

        public event Action<SceneNodeBase> Refreshed;
        public event EventHandler<ISceneNode> ParentNodeChanged;
        protected event EventHandler TransformationChanged;

        event EventHandler ITransformer.TransformationChanged
        {
            add => TransformationChanged += value;
            remove => TransformationChanged -= value;
        }

        public SceneNodeBase()
        {
            _childrenNodes = new ObservableList<ISceneNode>();
            Children = new ReadOnlyObservableList<ISceneNode>(_childrenNodes);

            _transformation = Transformation.Identity;
        }

        public SceneNodeBase(ISceneNode parentNode)
            : this()
        {
            SetParent(parentNode, Referential.Local);
        }

        public override void Initialize()
        {
            if (IsInitialized || Parent == null)
                return;

            if (Parent.ParentQueue().SelectMany(x => x.Components).AnyOfType(out SceneNodeBase parentNode))
                SetParent(parentNode, Referential.Local);
        }

        public override void Dispose()
        {
            foreach (ISceneNode childNode in _childrenNodes.ToArray())
                childNode.SetParent(null);

            SetParent(null);
        }

        public void SetParent(ISceneNode parent, Referential childStaticReferential = Referential.World)
        {
            if (IsInitialized && ParentNode == parent)
                return;

            ParentNode?.UnlinkChild(this);
            ParentNode = parent;
            ParentNode?.LinkChild(this, childStaticReferential);

            _isRoot = ParentNode == null;
            Refresh(childStaticReferential);

            ParentNodeChanged?.Invoke(this, parent);
        }

        public void MakesRoot()
        {
            SetParent(null, Referential.Local);
        }

        public Vector2 GetPosition(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return _position;
                case Referential.Local: return _transformation.Translation;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetRotation(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return _rotation;
                case Referential.Local: return _transformation.Rotation;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetScale(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return _scale;
                case Referential.Local: return _transformation.Scale;
                default:
                    throw new NotSupportedException();
            }
        }

        public float GetDepth(Referential referential)
        {
            switch (referential)
            {
                case Referential.World: return _depth;
                case Referential.Local: return _localDepth;
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

        protected void SetWorldDepth(float value)
        {
            if (ParentNode != null)
                _localDepth = value - ParentNode.Depth;
            else
                _localDepth = value;
        }

        public bool Represent(IRepresentative<ISceneNode> other)
        {
            return this == other;
        }
        
        void ISceneNode.Refresh() => Refresh();
        protected virtual void Refresh() => Refresh(Referential.Local);

        protected void Refresh(Referential childStaticReferential)
        {
            if (ParentNode == null)
            {
                _position = _transformation.Translation;
                _rotation = _transformation.Rotation;
                _scale = _transformation.Scale;
                _depth = _localDepth;
                Matrix = _transformation.Matrix;
            }
            else
            {
                switch (childStaticReferential)
                {
                    case Referential.Local:
                        _position = ParentNode.Matrix * _transformation.Translation;
                        _rotation = MathHelper.WrapAngle(ParentNode.Rotation + _transformation.Rotation);
                        _scale = ParentNode.Scale * _transformation.Scale;
                        _depth = ParentNode.Depth + _localDepth;
                        break;
                    case Referential.World:
                        _transformation.RefreshMatrix(ParentNode.Matrix.Inverse * _position, _rotation - ParentNode.Rotation, _scale / ParentNode.Scale);
                        _localDepth = _depth - ParentNode.Depth;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                Matrix = ParentNode.Matrix * _transformation.Matrix;
            }

            foreach (ISceneNode childNode in _childrenNodes)
                childNode.Refresh();

            Refreshed?.Invoke(this);
            TransformationChanged?.Invoke(this, EventArgs.Empty);
        }

        void ISceneNode.LinkChild(ISceneNode child, Referential childStaticReferential)
        {
            if (_childrenNodes.Contains(child))
                throw new InvalidOperationException("Parent already have this scene node as a child !");

            if (!child.ParentNode.Represent(this))
                child.SetParent(this, childStaticReferential);
            else
                _childrenNodes.Add(child);
        }

        void ISceneNode.UnlinkChild(ISceneNode child)
        {
            _childrenNodes.Remove(child);
        }

        public Vector2 Transform(Vector2 position) => new Transformation(_position, _rotation, _scale).Transform(position);
        public Vector2 InverseTransform(Vector2 position) => new Transformation(_position, _rotation, _scale).InverseTransform(position);
        public Transformation Transform(Transformation transformation) => new Transformation(_position, _rotation, _scale).Transform(transformation);
        public Transformation InverseTransform(Transformation transformation) => new Transformation(_position, _rotation, _scale).InverseTransform(transformation);
    }
}