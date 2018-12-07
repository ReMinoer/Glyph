using Glyph.Core.Base;

namespace Glyph.Core
{
    public class ReadOnlySceneNode : ReadOnlySceneNodeBase
    {
        protected override ISceneNode SceneNode { get; }

        public ReadOnlySceneNode(ISceneNode sceneNode)
        {
            SceneNode = sceneNode;
        }
    }
}