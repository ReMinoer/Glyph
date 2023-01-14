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
        private ISceneNode _anchorNode;
        private ISceneNode _scaleAnchorNode;
        private ITransformation _anchorTransformation;
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
        
        private void Refresh(object sender, EventArgs args) => Refresh();
        protected override void Refresh()
        {
            if (_anchorNode == null || !_projectionManager.SceneRoots.Contains(_anchorNode.RootNode(), new RepresentativeEqualityComparer<ISceneNode>()))
            {
                _transformation = Transformation.Identity;
                RefreshFrom(Referential.Local);
                return;
            }
            
            ITransformation worldTransformation = GetProjectionTransformation(_anchorNode, _anchorTransformation, _intermediaryTransformers);
            if (worldTransformation is null)
            {
                _transformation = Transformation.Identity;
                RefreshFrom(Referential.Local);
                return;
            }

            ITransformation scaleWorldTransformation = _scaleAnchorNode is null ? null : GetProjectionTransformation(_scaleAnchorNode, null, _scaleIntermediaryTransformers);

            _position = worldTransformation.Translation;
            _rotation = IgnoreRotation ? ParentNode?.Rotation ?? 0 : worldTransformation.Rotation;
            _scale = IgnoreScale ? ParentNode?.Scale ?? 1 : scaleWorldTransformation?.Scale ?? worldTransformation.Scale;

            RefreshFrom(Referential.World);
        }

        private ITransformation GetProjectionTransformation(ISceneNode anchorNode, ITransformation anchorTransformation, ISet<ITransformer> projectionTransformers)
        {
            foreach (ITransformer previousTransformer in projectionTransformers)
                UnsubscribeToTransformationChanged(previousTransformer);
            projectionTransformers.Clear();

            ProjectionManager.IOptionsController<ITransformation> projectionController = _projectionManager.ProjectFrom(anchorNode, anchorTransformation ?? anchorNode).To(this);
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

            if (_scaleAnchorNode != null)
                UnsubscribeToTransformationChanged(_scaleAnchorNode);
            if (_anchorTransformation != null)
                UnsubscribeToTransformationChanged(_anchorTransformation);
            if (_anchorNode != null)
                UnsubscribeToTransformationChanged(_anchorNode);

            base.Dispose();
        }
    }
}