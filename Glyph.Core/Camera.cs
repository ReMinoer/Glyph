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
        public ISceneNode SceneNode { get; }

        private ITransformation Transformation { get; set; }
        public ITransformation RenderTransformation { get; private set; }

        private float _zoom;
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

        public Vector2 Position
        {
            get => _sceneNode.Position;
            set => _sceneNode.Position = value;
        }

        public float Rotation
        {
            get => _sceneNode.Rotation;
            set => _sceneNode.Rotation = value;
        }

        Vector2 ITransformation.Translation => Transformation.Translation;
        float ITransformation.Rotation => Transformation.Rotation;
        float ITransformation.Scale => Transformation.Scale;
        Matrix3X3 ITransformation.Matrix => Transformation.Matrix;

        public event EventHandler TransformationChanged;

        public Camera()
        {
            Components.Add(_sceneNode = new SceneNode());

            SceneNode = new ReadOnlySceneNode(_sceneNode);
            Zoom = 1f;

            _sceneNode.Refreshed += OnSceneNodeRefreshed;
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public Vector2 Transform(Vector2 position) => Transformation.Transform(position);
        public Vector2 InverseTransform(Vector2 position) => Transformation.InverseTransform(position);
        public ITransformation Transform(ITransformation transformation) => Transformation.Transform(transformation);
        public ITransformation InverseTransform(ITransformation transformation) => Transformation.InverseTransform(transformation);

        private void Refresh()
        {
            Transformation = new Transformation(_sceneNode.Position, -_sceneNode.Rotation, 1 / Zoom);
            RenderTransformation = new Transformation(Vector2.Zero, _sceneNode.Rotation, Zoom).Transform(new Transformation(-_sceneNode.Position, 0, 1));

            TransformationChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnSceneNodeRefreshed(SceneNodeBase obj) => Refresh();
    }
}