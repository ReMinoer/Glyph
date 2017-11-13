using Glyph.Composition;

namespace Glyph.Core
{
    public class PositionBinding : GlyphComponent, IEnableable, IUpdate
    {
        private readonly SceneNode _sceneNode;
        public bool Enabled { get; set; }
        public ISceneNode Binding { get; set; }

        public PositionBinding(SceneNode sceneNode)
        {
            Enabled = true;
            
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