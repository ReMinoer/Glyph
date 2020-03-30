using System.Collections.Generic;

namespace Glyph.Tools.Transforming
{
    public interface IMultiModeTransformationController
    {
        IReadOnlyList<ITransformationController> Modes { get; }
    }

    public interface IMultiModeAnchoredTransformationController : IMultiModeTransformationController
    {
        new IReadOnlyList<IAnchoredTransformationController> Modes { get; }
    }
}