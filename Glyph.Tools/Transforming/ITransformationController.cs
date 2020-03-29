namespace Glyph.Tools.Transforming
{
    public interface ITransformationController : IAnchoredController
    {
        bool OrientedReferential { get; }
        IPositionController PositionController { get; }
        IRotationController RotationController { get; }
        IScaleController ScaleController { get; }
    }
}