using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;
using Niddle.Attributes;

namespace Glyph.Tools.Transforming
{
    public class AdvancedScaleHandle : AdvancedHandleBase<IAnchoredScaleController>
    {
        private float _startScale;
        private float _lastScale;
        private Vector2 _relativeGrabPosition;

        [Resolvable]
        public IUndoRedoStack UndoRedoStack { get; set; }

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
            float newScale = _startScale * MathHelper.Max(MathHelper.Max(scaleRatio.X, scaleRatio.Y), 0.1f);

            EditedObject.Scale = newScale;
            _lastScale = newScale;
        }

        protected override void OnReleased()
        {
            base.OnReleased();

            IAnchoredScaleController editedObject = EditedObject;
            float newScale = _lastScale;
            float previousScale = _startScale;

            UndoRedoStack.Push($"Set scale to {newScale}.",
                () => editedObject.Scale = newScale,
                () => editedObject.Scale = previousScale);
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            EditedObject.Scale = _startScale;
        }
    }
}