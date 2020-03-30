using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class AdvancedScaleHandle : AdvancedHandleBase<IAnchoredScaleController>
    {
        private float _startScale;
        private Vector2 _relativeGrabPosition;

        public AdvancedScaleHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            base.OnGrabbed(cursorPosition);

            _startScale = EditedObject.Scale;
            _relativeGrabPosition = ProjectToTargetScene(cursorPosition - _sceneNode.Position);
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            projectedCursorPosition -= _relativeGrabPosition;

            Vector2 scaleRatio = (projectedCursorPosition - _sceneNode.ParentNode.Position) / _sceneNode.LocalPosition.Rotate(_sceneNode.ParentNode.Rotation);
            EditedObject.Scale = _startScale * MathHelper.Max(MathHelper.Max(scaleRatio.X, scaleRatio.Y), 0.1f);
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            EditedObject.Scale = _startScale;
        }
    }
}