using Glyph.Composition.Modelization;
using Glyph.Core;
using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class TransformableDataController : IAnchoredTransformationController
    {
        public ISceneNode Anchor { get; }
        public IAnchoredPositionController PositionController { get; }
        public IAnchoredRotationController RotationController { get; }
        public IAnchoredScaleController ScaleController { get; }
        public bool OrientedReferential { get; }

        IPositionController ITransformationController.PositionController => PositionController;
        IRotationController ITransformationController.RotationController => RotationController;
        IScaleController ITransformationController.ScaleController => ScaleController;

        public TransformableDataController(IGlyphData data)
        {
            var anchoredController = data as IAnchoredController;
            Anchor = anchoredController?.Anchor ?? data.BindedObject.GetSceneNode();

            if (data is ITransformationController transformationController)
            {
                PositionController = new AnchoredPositionController(transformationController.PositionController, Anchor);
                RotationController = new AnchoredRotationController(transformationController.RotationController, Anchor);
                ScaleController = new AnchoredScaleController(transformationController.ScaleController, Anchor);
                OrientedReferential = transformationController.OrientedReferential;
            }
            else
            {
                PositionController = data is IPositionController positionController ? new AnchoredPositionController(positionController, Anchor) : null;
                RotationController = data is IRotationController rotationController ? new AnchoredRotationController(rotationController, Anchor) : null;
                ScaleController = data is IScaleController scaleController ? new AnchoredScaleController(scaleController, Anchor) : null;
            }
        }

        public class AnchoredPositionController : IAnchoredPositionController
        {
            private readonly IPositionController _controller;

            public bool IsLocalPosition => _controller.IsLocalPosition;
            public Vector2 Position
            {
                get => _controller.Position;
                set => _controller.Position = value;
            }

            public ISceneNode Anchor { get; }

            public AnchoredPositionController(IPositionController controller, ISceneNode anchor)
            {
                _controller = controller;
                Anchor = anchor;
            }
        }

        public class AnchoredRotationController : IAnchoredRotationController
        {
            private readonly IRotationController _controller;

            public bool IsLocalRotation => _controller.IsLocalRotation;
            public float Rotation
            {
                get => _controller.Rotation;
                set => _controller.Rotation = value;
            }

            public ISceneNode Anchor { get; }

            public AnchoredRotationController(IRotationController controller, ISceneNode anchor)
            {
                _controller = controller;
                Anchor = anchor;
            }
        }

        public class AnchoredScaleController : IAnchoredScaleController
        {
            private readonly IScaleController _controller;

            public bool IsLocalScale => _controller.IsLocalScale;
            public float Scale
            {
                get => _controller.Scale;
                set => _controller.Scale = value;
            }

            public ISceneNode Anchor { get; }

            public AnchoredScaleController(IScaleController controller, ISceneNode anchor)
            {
                _controller = controller;
                Anchor = anchor;
            }
        }
    }
}