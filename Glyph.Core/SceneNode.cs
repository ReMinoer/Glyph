using System;
using Glyph.Composition;
using Glyph.Core.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    [SinglePerParent]
    public class SceneNode : SceneNodeBase, IWritableSceneNodeComponent
    {
        public Transformation LocalTransformation
        {
            get => _transformation;
            set
            {
                if (_transformation.Equals(value))
                    return;

                _transformation = value;
                Refresh();
            }
        }

        public Vector2 LocalPosition
        {
            get => LocalTransformation.Translation;
            set
            {
                if (LocalPosition.EpsilonEquals(value))
                    return;

                LocalTransformation.Translation = value;
                Refresh();
            }
        }

        public float LocalRotation
        {
            get => LocalTransformation.Rotation;
            set
            {
                if (LocalRotation.EpsilonEquals(value))
                    return;

                LocalTransformation.Rotation = value;
                Refresh();
            }
        }

        public float LocalScale
        {
            get => LocalTransformation.Scale;
            set
            {
                if (LocalScale.EpsilonEquals(value))
                    return;

                LocalTransformation.Scale = value;
                Refresh();
            }
        }

        public Matrix3X3 LocalMatrix => LocalTransformation.Matrix;

        public Transformation Transformation
        {
            get => new Transformation(Position, Rotation, Scale);
            set => SetValues(value.Translation, value.Rotation, value.Scale, null, Referential.World);
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                SetWorldPosition(value);
                Refresh();
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                SetWorldRotation(value);
                Refresh();
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                SetWorldScale(value);
                Refresh();
            }
        }
        
        public SceneNode()
        {
        }

        public SceneNode(ISceneNode parentNode)
            : base(parentNode)
        {
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
            Refresh();
        }

        private void SetWorldPosition(Vector2 value)
        {
            if (ParentNode != null)
                value = ParentNode.Matrix.Inverse * value;

            if (LocalTransformation.Translation.Equals(value))
                return;

            LocalTransformation.Translation = value;
        }

        private void SetWorldRotation(float value)
        {
            if (ParentNode != null)
                value = value - ParentNode.Rotation;

            if (LocalTransformation.Rotation.Equals(value))
                return;

            LocalTransformation.Rotation = value;
        }

        private void SetWorldScale(float value)
        {
            if (ParentNode != null)
                value = value / ParentNode.Scale;

            if (LocalTransformation.Scale.Equals(value))
                return;

            LocalTransformation.Scale = value;
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

        public void Flip(Axes axes)
        {
            LocalTransformation.Flip(axes);
            Refresh();
        }
    }
}