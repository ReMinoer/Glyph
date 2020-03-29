using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Microsoft.Xna.Framework;

namespace Glyph.Tools.Transforming
{
    public class AdvancedRotationHandle : AdvancedHandleBase<IRotationController>
    {
        private float _startRotation;
        private float _relativeRotation;

        public AdvancedRotationHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override void OnGrabbed(Vector2 cursorPosition)
        {
            base.OnGrabbed(cursorPosition);

            _startRotation = EditedObject.Rotation;

            Vector2 pivotPosition = _sceneNode.ParentNode.Position;
            _relativeRotation = (ProjectToTargetScene(cursorPosition) - pivotPosition).ToRotation().GetValueOrDefault();
        }
        
        protected override void OnDragging(Vector2 projectedCursorPosition)
        {
            Vector2 pivotPosition = _sceneNode.ParentNode.Position;
            float? cursorRotation = (projectedCursorPosition - pivotPosition).ToRotation();
            if (!cursorRotation.HasValue)
                return;
            
            EditedObject.Rotation = _startRotation + cursorRotation.Value - _relativeRotation;
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            EditedObject.Rotation = _startRotation;
        }
    }
}