using Glyph.Math.Shapes;

namespace Glyph.Tools.Transforming
{
    public class AnchoredRectangleController : IAnchoredRectangleController
    {
        private readonly IRectangleController _controller;
        public ISceneNode Anchor { get; }
        
        public bool IsLocalRectangle => _controller.IsLocalRectangle;
        public TopLeftRectangle Rectangle
        {
            get => _controller.Rectangle;
            set => _controller.Rectangle = value;
        }
        public TopLeftRectangle LiveRectangle
        {
            get => _controller.LiveRectangle;
            set => _controller.LiveRectangle = value;
        }

        public AnchoredRectangleController(IRectangleController controller, ISceneNode anchor)
        {
            _controller = controller;
            Anchor = anchor;
        }
    }
}