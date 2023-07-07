using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public interface IPositionController
    {
        bool IsLocalPosition { get; }
        Vector2 Position { get; set; }
        Vector2 LivePosition { get; set; }
    }

    public interface IRotationController
    {
        bool IsLocalRotation { get; }
        float Rotation { get; set; }
        float LiveRotation { get; set; }
    }

    public interface IScaleController
    {
        bool IsLocalScale { get; }
        float Scale { get; set; }
        float LiveScale { get; set; }
    }

    public interface ITransformationController
    {
        bool OrientedReferential { get; }
        IPositionController PositionController { get; }
        IRotationController RotationController { get; }
        IScaleController ScaleController { get; }
    }

    public interface IAnchoredController
    {
        ISceneNode Anchor { get; }
    }

    public interface IAnchoredPositionController : IPositionController, IAnchoredController
    {
    }

    public interface IAnchoredRotationController : IRotationController, IAnchoredController
    {
    }

    public interface IAnchoredScaleController : IScaleController, IAnchoredController
    {
    }

    public interface IAnchoredTransformationController : ITransformationController, IAnchoredController
    {
        new IAnchoredPositionController PositionController { get; }
        new IAnchoredRotationController RotationController { get; }
        new IAnchoredScaleController ScaleController { get; }
    }

    public interface IRectangleController
    {
        bool IsLocalRectangle { get; }
        TopLeftRectangle Rectangle { get; set; }
        TopLeftRectangle LiveRectangle { get; set; }
    }

    public interface IAnchoredRectangleController : IRectangleController, IAnchoredController
    {
    }
}