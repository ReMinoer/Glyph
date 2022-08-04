using Glyph.Core;
using Glyph.Tools.Transforming.Base;
using Glyph.Tools.UndoRedo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Niddle.Attributes;

namespace Glyph.Tools.Transforming
{
    public class AdvancedRotationHandle : AdvancedHandleBase<IAnchoredRotationController>
    {
        private float _startRotation;
        private float _lastRotation;
        private float _relativeRotation;

        [Resolvable]
        public IUndoRedoStack UndoRedoStack { get; set; }

        public AdvancedRotationHandle(GlyphResolveContext context, ProjectionManager projectionManager)
            : base(context, projectionManager)
        {
        }

        protected override MouseCursor Cursor => MouseCursor.Hand;

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

            var newRotation = _startRotation + cursorRotation.Value - _relativeRotation;

            EditedObject.Rotation = newRotation;
            _lastRotation = newRotation;
        }

        protected override void OnReleased()
        {
            base.OnReleased();

            IAnchoredRotationController editedObject = EditedObject;
            float newRotation = _lastRotation;
            float previousRotation = _startRotation;

            if (EditedObject.IsLocalRotation)
                newRotation = editedObject.Anchor.Rotation / editedObject.Anchor.LocalRotation;

            if (newRotation.EpsilonEquals(previousRotation))
                return;

            UndoRedoStack.Push($"Set rotation to {newRotation}.",
                () => editedObject.Rotation = newRotation,
                () => editedObject.Rotation = previousRotation);
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            EditedObject.Rotation = _startRotation;
        }
    }
}