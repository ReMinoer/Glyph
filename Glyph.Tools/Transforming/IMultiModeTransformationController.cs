using System.Collections.Generic;

namespace Glyph.Tools.Transforming
{
    public interface IMultiModeTransformationController
    {
        IReadOnlyList<ITransformationController> Modes { get; }
    }
}