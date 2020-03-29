using System.Collections.Generic;
using Diese.Collections;
using Glyph.Core;

namespace Glyph.Tools.Transforming
{
    public class MultiModeTransformationController : IMultiModeTransformationController
    {
        public IReadOnlyList<ITransformationController> Modes { get; }

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