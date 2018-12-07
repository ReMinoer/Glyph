using Glyph.Composition;
using Glyph.Math;
using Microsoft.Xna.Framework;

namespace Glyph.Core
{
    public class Camera : GlyphContainer, ICamera
    {
        private readonly SceneNode _sceneNode;
        
        private ITransformation Transformation
            => new Transformation(_sceneNode.Position, -_sceneNode.Rotation, 1 / Zoom);
        public ITransformation RenderTransformation
            => new Transformation(Vector2.Zero, _sceneNode.Rotation, Zoom)
                .Transform(new Transformation(-_sceneNode.Position, 0, 1));

        public float Zoom { get; set; }

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

        public Camera()
        {
            Zoom = 1f;
            Components.Add(_sceneNode = new SceneNode());
        }

        public override void Initialize()
        {
            _sceneNode.Initialize();
        }

        public Vector2 Transform(Vector2 position) => Transformation.Transform(position);
        public Vector2 InverseTransform(Vector2 position) => Transformation.InverseTransform(position);
        public Transformation Transform(Transformation transformation) => Transformation.Transform(transformation);
        public Transformation InverseTransform(Transformation transformation) => Transformation.InverseTransform(transformation);

        void IFlipable.Flip(Axes axes) => Transformation.Flip(axes);
    }
}