using Glyph.Math.Shapes;
using Microsoft.Xna.Framework;

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
        public Vector2 LiveRectanglePosition
        {
            get => _controller.LiveRectanglePosition;
            set => _controller.LiveRectanglePosition = value;
        }
        public Vector2 LiveRectangleSize
        {
            get => _controller.LiveRectangleSize;
            set => _controller.LiveRectangleSize = value;
        }

        public AnchoredRectangleController(IRectangleController controller, ISceneNode anchor)
        {
            _controller = controller;
            Anchor = anchor;
        }
    }
}