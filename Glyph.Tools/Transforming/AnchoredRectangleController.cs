using Glyph.Math.Shapes;

namespace Glyph.Tools.Transforming
{
    public class AnchoredRectangleController : IAnchoredRectangleController
    {
        public ISceneNode Anchor { get; }

        private readonly IRectangleController _controller;
        public TopLeftRectangle Rectangle
        {
            get => _controller.Rectangle;
            set => _controller.Rectangle = value;
        }

        public bool IsLocalRectangle => _controller.IsLocalRectangle;

        public AnchoredRectangleController(IRectangleController controller, ISceneNode anchor)
        {
            _controller = controller;
            Anchor = anchor;
        }
    }
}