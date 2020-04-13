using Glyph.Composition;

namespace Glyph.Core
{
    public class PositionBinding : GlyphComponent, IUpdate
    {
        private readonly SceneNode _sceneNode;
        public ISceneNode Binding { get; set; }

        public PositionBinding(SceneNode sceneNode)
        {
            _sceneNode = sceneNode;
        }

        public void Update(ElapsedTime elapsedTime)
        {
            if (!Enabled || Binding == null)
                return;
            
            _sceneNode.LocalPosition = Binding.Project(_sceneNode.ParentNode);
        }
    }
}