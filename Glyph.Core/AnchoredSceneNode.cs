using System;
using System.Collections.Generic;
using System.Linq;
using Glyph.Composition;
using Glyph.Core.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    [SinglePerParent]
    public class AnchoredSceneNode : SceneNodeBase
    {
        private readonly ProjectionManager _projectionManager;
        private readonly HashSet<ITransformer> _intermediaryTransformers = new HashSet<ITransformer>();
        private readonly HashSet<ITransformer> _scaleIntermediaryTransformers = new HashSet<ITransformer>();

        private bool _ignoreRotation;
        public bool IgnoreRotation
        {
            get => _ignoreRotation;
            set
            {
                if (_ignoreRotation == value)
                    return;

                _ignoreRotation = value;
                Refresh();
            }
        }

        private bool _ignoreScale;
        public bool IgnoreScale
        {
            get => _ignoreScale;
            set
            {
                if (_ignoreScale == value)
                    return;

                _ignoreScale = value;
                Refresh();
            }
        }

        private ISceneNode _anchorNode;
        public ISceneNode AnchorNode
        {
            get => _anchorNode;
            set
            {
                if (_anchorNode == value)
                    return;

                if (_anchorNode != null)
                    UnsubscribeToTransformationChanged(_anchorNode);

                _anchorNode = value;
                Refresh();

                if (_anchorNode != null)
                    SubscribeToTransformationChanged(_anchorNode);
            }
        }

        private ISceneNode _scaleAnchorNode;
        public ISceneNode ScaleAnchorNode
        {
            get => _scaleAnchorNode;
            set
            {
                if (_scaleAnchorNode == value)
                    return;

                if (_scaleAnchorNode != null)
                    UnsubscribeToTransformationChanged(_scaleAnchorNode);

                _scaleAnchorNode = value;
                Refresh();

                if (_scaleAnchorNode != null)
                    SubscribeToTransformationChanged(_scaleAnchorNode);
            }
        }

        private IView _anchorView;
        public IView AnchorView
        {
            get => _anchorView;
            set
            {
                if (_anchorView == value)
                    return;

                if (_anchorView != null)
                    UnsubscribeToTransformationChanged(_anchorView);

                _anchorView = value;
                Refresh();

                if (_anchorView != null)
                    SubscribeToTransformationChanged(_anchorView);
            }
        }

        private IView _scaleAnchorView;
        public IView ScaleAnchorView
        {
            get => _scaleAnchorView;
            set
            {
                if (_scaleAnchorView == value)
                    return;

                if (_scaleAnchorView != null)
                    UnsubscribeToTransformationChanged(_scaleAnchorView);

                _scaleAnchorView = value;
                Refresh();

                if (_scaleAnchorView != null)
                    SubscribeToTransformationChanged(_scaleAnchorView);
            }
        }

        private ITransformation _anchorTransformation;
        public ITransformation AnchorTransformation
        {
            get => _anchorTransformation;
            set
            {
                if (_anchorTransformation == value)
                    return;

                if (_anchorTransformation != null)
                    UnsubscribeToTransformationChanged(_anchorTransformation);

                _anchorTransformation = value;
                Refresh();

                if (_anchorTransformation != null)
                    SubscribeToTransformationChanged(_anchorTransformation);
            }
        }

        private ITransformation _scaleAnchorTransformation;
        public ITransformation ScaleAnchorTransformation
        {
            get => _scaleAnchorTransformation;
            set
            {
                if (_scaleAnchorTransformation == value)
                    return;

                if (_scaleAnchorTransformation != null)
                    UnsubscribeToTransformationChanged(_scaleAnchorTransformation);

                _scaleAnchorTransformation = value;
                Refresh();

                if (_scaleAnchorTransformation != null)
                    SubscribeToTransformationChanged(_scaleAnchorTransformation);
            }
        }

        public Func<ProjectionManager.IOptionsController<ITransformation>, ProjectionManager.IOptionsController<ITransformation>> ProjectionConfiguration { get; set; }

        public Transformation LocalTransformation => _transformation;
        public Vector2 LocalPosition => LocalTransformation.Translation;
        public float LocalRotation => LocalTransformation.Rotation;
        public float LocalScale => LocalTransformation.Scale;
        public Matrix3X3 LocalMatrix => LocalTransformation.Matrix;
        public Transformation Transformation => new Transformation(Position, Rotation, Scale);
        public Vector2 Position => _position;
        public float Rotation => _rotation;
        public float Scale => _scale;

        public AnchoredSceneNode(ProjectionManager projectionManager)
        {
            _projectionManager = projectionManager;
        }

        public AnchoredSceneNode(ISceneNodeComponent parentNode, ProjectionManager projectionManager)
            : base(parentNode)
        {
            _projectionManager = projectionManager;
        }
        
        protected override void Refresh()
        {
            ITransformation worldTransformation = GetWorldTransformation(AnchorNode, AnchorView, AnchorTransformation, _intermediaryTransformers);
            if (worldTransformation is null)
            {
                _transformation = Transformation.Identity;
                RefreshFrom(Referential.Local);
                return;
            }

            ITransformation scaleWorldTransformation = GetWorldTransformation(ScaleAnchorNode, ScaleAnchorView, ScaleAnchorTransformation, _scaleIntermediaryTransformers);

            _position = worldTransformation.Translation;
            _rotation = IgnoreRotation ? ParentNode?.Rotation ?? 0 : worldTransformation.Rotation;
            _scale = IgnoreScale ? ParentNode?.Scale ?? 1 : scaleWorldTransformation?.Scale ?? worldTransformation.Scale;

            RefreshFrom(Referential.World);
        }

        private ITransformation GetWorldTransformation(ISceneNode anchorNode, IView anchorView, ITransformation anchorTransformation, ISet<ITransformer> projectionTransformers)
        {
            ITransformation worldTransformation = null;
            if (anchorNode != null && _projectionManager.SceneRoots.Contains(anchorNode.RootNode(), new RepresentativeEqualityComparer<ISceneNode>()))
            {
                worldTransformation = GetProjectionTransformation(x => _projectionManager.ProjectFrom(anchorNode, anchorTransformation ?? anchorNode), projectionTransformers);
            }
            else if (anchorView != null && _projectionManager.Views.Contains(anchorView))
            {
                worldTransformation = GetProjectionTransformation(x => _projectionManager.ProjectFrom(anchorView, anchorTransformation ?? Transformation.Identity), projectionTransformers);
            }

            return worldTransformation;
        }

        private ITransformation GetProjectionTransformation(
            Func<ProjectionManager, ProjectionManager.IProjectionController<ITransformation>> projectionFunc,
            ISet<ITransformer> projectionTransformers)
        {
            foreach (ITransformer previousTransformer in projectionTransformers)
                UnsubscribeToTransformationChanged(previousTransformer);
            projectionTransformers.Clear();

            ProjectionManager.IOptionsController<ITransformation> projectionController = projectionFunc(_projectionManager).To(this);
            if (ProjectionConfiguration != null)
                projectionController = ProjectionConfiguration(projectionController);

            Projection<ITransformation> projection = projectionController.FirstOrDefault();
            if (projection == null)
            {
                _transformation = Transformation.Identity;
                RefreshFrom(Referential.Local);
                return null;
            }

            foreach (ITransformer newTransformer in projection.TransformerPath)
                if (projectionTransformers.Add(newTransformer))
                    SubscribeToTransformationChanged(newTransformer);

            return projection.Value;
        }

        private void SubscribeToTransformationChanged(ITransformer transformer)
        {
            transformer.TransformationChanged += OnProjectionTransformationChanged;
        }

        private void UnsubscribeToTransformationChanged(ITransformer transformer)
        {
            transformer.TransformationChanged -= OnProjectionTransformationChanged;
        }

        private void OnProjectionTransformationChanged(object sender, EventArgs e) => Refresh();

        public override void Dispose()
        {
            foreach (ITransformer transformer in _scaleIntermediaryTransformers)
                UnsubscribeToTransformationChanged(transformer);
            foreach (ITransformer transformer in _intermediaryTransformers)
                UnsubscribeToTransformationChanged(transformer);

            if (ScaleAnchorNode != null)
                UnsubscribeToTransformationChanged(ScaleAnchorNode);
            if (AnchorTransformation != null)
                UnsubscribeToTransformationChanged(AnchorTransformation);
            if (AnchorNode != null)
                UnsubscribeToTransformationChanged(AnchorNode);

            base.Dispose();
        }
    }
}