using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public interface IPositionController
    {
        Vector2 Position { get; set; }
    }

    public interface IRotationController
    {
        float Rotation { get; set; }
    }

    public interface IScaleController
    {
        float Scale { get; set; }
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
        TopLeftRectangle Rectangle { get; set; }
    }

    public interface IAnchoredRectangleController : IRectangleController, IAnchoredController
    {
    }
}