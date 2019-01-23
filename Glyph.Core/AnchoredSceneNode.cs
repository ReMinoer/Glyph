using System;
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
        private ISceneNodeComponent _anchorNode;
        private ITransformer[] _projectionPath;

        public ISceneNodeComponent AnchorNode
        {
            get => _anchorNode;
            set
            {
                if (_anchorNode == value)
                    return;

                if (_anchorNode != null)
                    _anchorNode.TransformationChanged -= OnAnchorNodeTransformationChanged;

                _anchorNode = value;
                Refresh();

                if (_anchorNode != null)
                    _anchorNode.TransformationChanged += OnAnchorNodeTransformationChanged;
            }
        }

        public ITransformer[] TransformerPath
        {
            get => _projectionPath;
            set
            {
                if (_projectionPath == value || _projectionPath != null && value != null && _projectionPath.SequenceEqual(value))
                    return;

                if (_projectionPath != null)
                    foreach (ITransformer transformer in _projectionPath)
                    {
                        transformer.TransformationChanged -= Refresh;
                    }

                _projectionPath = value;

                if (_projectionPath != null)
                    foreach (ITransformer transformer in _projectionPath)
                    {
                        transformer.TransformationChanged += Refresh;
                    }

                Refresh();
            }
        }
        
        public Func<ProjectionManager.IOptionsController<Transformation>, ProjectionManager.IOptionsController<Transformation>> ProjectionConfiguration { get; set; }

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
            if (_anchorNode == null || !_projectionManager.SceneRoots.Contains(_anchorNode.RootNode(), new RepresentativeEqualityComparer<ISceneNode>()))
            {
                _transformation = Transformation.Identity;
                Refresh(Referential.Local);
                return;
            }

            ProjectionManager.IOptionsController<Transformation> projectionController = _projectionManager.ProjectFrom(_anchorNode).To(this);
            if (ProjectionConfiguration != null)
                projectionController = ProjectionConfiguration(projectionController);
            
            Transformation worldTransformation = projectionController.First().Value;
            //Transformation worldTransformation = projectionController.First(x => x.TransformerPath.SequenceEqual(TransformerPath)).Value;

            _position = worldTransformation.Translation;
            _rotation = worldTransformation.Rotation;
            _scale = worldTransformation.Scale;

            Refresh(Referential.World);
        }

        private void Refresh(object sender, EventArgs args) => Refresh();
        private void OnAnchorNodeTransformationChanged(object sender, EventArgs e) => Refresh();

        public override void Dispose()
        {
            if (_anchorNode != null)
                _anchorNode.TransformationChanged -= OnAnchorNodeTransformationChanged;

            base.Dispose();
        }
    }
}