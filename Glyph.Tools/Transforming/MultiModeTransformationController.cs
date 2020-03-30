using System.Collections.Generic;
using Diese.Collections;
using Glyph.Core;

namespace Glyph.Tools.Transforming
{
    public class MultiModeTransformationController : IMultiModeAnchoredTransformationController
    {
        public IReadOnlyList<IAnchoredTransformationController> Modes { get; }
        IReadOnlyList<ITransformationController> IMultiModeTransformationController.Modes => Modes;

        public MultiModeTransformationController(IWritableSceneNode sceneNode)
        {
            Modes = new[]
            {
                new TransformationController(sceneNode, false),
                new TransformationController(sceneNode, true)
            }.AsReadOnly();
        }
    }
}