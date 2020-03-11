using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Glyph.Tools.Transforming
{
    public class AdvancedPositionHandle : AdvancedHandleBase
    {
        private Vector2 _startPosition;
        private Vector2 _relativeGrabPosition;

        public Axes Axes { get; set; } = Axes.Both;

        public AdvancedPositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            base.OnGrabbed(cursorPosition);

            _startPosition = EditedObject.Position;
            _relativeGrabPosition = ProjectToTargetScene(cursorPosition - _sceneNode.Position + _sceneNode.LocalPosition);
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            projectedCursorPosition -= _relativeGrabPosition;

            switch (Axes)
            {
                case Axes.Both:
                    EditedObject.Position = projectedCursorPosition;
                    break;
                case Axes.Horizontal:
                    EditedObject.Position = _startPosition.SetX(projectedCursorPosition.X);
                    break;
                case Axes.Vertical:
                    EditedObject.Position = _startPosition.SetY(projectedCursorPosition.Y);
                    break;
            }
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            EditedObject.Position = _startPosition;
        }
    }
}