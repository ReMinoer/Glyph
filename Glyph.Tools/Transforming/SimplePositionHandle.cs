using Glyph.Core;
using Glyph.Graphics.Shapes;
using Glyph.Math;
using Glyph.Math.Shapes;
using Glyph.Tools.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class SimplePositionHandle : SimpleHandleBase<FilledCircleSprite, IAnchoredPositionController>
    {
        private Vector2 _startPosition;
        private Vector2 _relativeGrabPosition;
        
        public Axes Axes { get; set; } = Axes.Both;
        protected override IArea Area => new Circle(_sceneNode.Position, _spriteSource.Radius);

        public SimplePositionHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
            _spriteSource.Radius = 50;
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
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

        protected override void OnReleased()
        {
        }

        protected override void OnCancelled()
        {
        }
    }
}