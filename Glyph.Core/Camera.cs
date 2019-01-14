using System;
using Glyph.Composition;
using Glyph.Core.Base;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class Camera : GlyphContainer, ICamera
    {
        // TODO: Update scene node parent when changing camera parent
        private readonly SceneNode _sceneNode;
        private float _zoom;
        
        private ITransformation Transformation { get; set; }
        public ITransformation RenderTransformation { get; private set; }

        public float Zoom
        {
            get => _zoom;
            set
            {
                if (_zoom.EpsilonEquals(value))
                    return;

                _zoom = value;
                Refresh();
            }
        }

        public Vector2 LocalPosition
        {
            get => _sceneNode.LocalPosition;
            set => _sceneNode.LocalPosition = value;
        }

        public float LocalRotation
        {
            get => _sceneNode.LocalRotation;
            set => _sceneNode.LocalRotation = value;
        }

        public Vector2 Position => _sceneNode.Position;
        public float Rotation => _sceneNode.Rotation;

        Vector2 ITransformation.Translation => Transformation.Translation;
        float ITransformation.Rotation => Transformation.Rotation;
        float ITransformation.Scale => Transformation.Scale;
        Matrix3X3 ITransformation.Matrix => Transformation.Matrix;

        public event EventHandler TransformationChanged;

        public Camera()
        {
            Components.Add(_sceneNode = new SceneNode());
            Zoom = 1f;

            _sceneNode.Refreshed += OnSceneNodeRefreshed;
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public Vector2 Transform(Vector2 position) => Transformation.Transform(position);
        public Vector2 InverseTransform(Vector2 position) => Transformation.InverseTransform(position);
        public Transformation Transform(Transformation transformation) => Transformation.Transform(transformation);
        public Transformation InverseTransform(Transformation transformation) => Transformation.InverseTransform(transformation);

        private void Refresh()
        {
            Transformation = new Transformation(_sceneNode.Position, -_sceneNode.Rotation, 1 / Zoom);
            RenderTransformation = new Transformation(Vector2.Zero, _sceneNode.Rotation, Zoom).Transform(new Transformation(-_sceneNode.Position, 0, 1));

            TransformationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnSceneNodeRefreshed(SceneNodeBase obj) => Refresh();

        void IFlipable.Flip(Axes axes) => Transformation.Flip(axes);
    }
}